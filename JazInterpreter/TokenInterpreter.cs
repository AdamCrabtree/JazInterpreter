using System;
using System.Collections.Generic;

namespace TokenInterpreter
{
    class TokenInterpreter
    {
        Stack<int> localStack = new Stack<int>();
        Stack<Dictionary<string, int>> variableStackFrame = new Stack<Dictionary<string, int>>();
        Stack<int> returnAddressStack = new Stack<int>();
        Dictionary<string, int> currentVariableDictionary = new Dictionary<string, int>();
        Dictionary<string, int> nextVariableDictionary = new Dictionary<string, int>();
        bool inBegin = false;
        bool inMethod = false;
        bool inRecursion = false;
        string lastCall = "";

        public void interpret(Dictionary<int, string[]> lineDictionary, Dictionary<string, int> labelsDict)
        {
            int i = 1, temp = 0;
            string[] line;
            string variable = "", label = "";

            while (i <= lineDictionary.Count)
            {
                label = "";
                lineDictionary.TryGetValue(i, out line);
                if (line[0].Equals("halt")){
                    break;
                }
                switch (line[0])
                {
                    case ("show"):
                        string printline = "";
                        for (int j = 1; j < line.Length; j++)
                        {
                            printline += line[j] + " ";
                        }
                        Console.WriteLine(printline.Trim(' '));
                        break;
                    case ("push"):
                        localStack.Push(Convert.ToInt32(line[1]));
                        break;
                    case ("lvalue"):
                        variable = line[1];
                        break;
                    case (":="):
                        if(inBegin)
                        {
                            if (currentVariableDictionary.ContainsKey(variable))
                            {
                                currentVariableDictionary[variable] = localStack.Pop();
                                if(inRecursion)
                                {
                                    nextVariableDictionary[variable] = currentVariableDictionary[variable];
                                }
                            }
                            else if(nextVariableDictionary.ContainsKey(variable))
                            {
                                nextVariableDictionary[variable] = localStack.Pop();
                            }
                            else
                            {
                                nextVariableDictionary.Add(variable, localStack.Pop());
                            }
                        }
                        else
                        {
                            if (currentVariableDictionary.ContainsKey(variable))
                            {
                                currentVariableDictionary[variable] = localStack.Pop();
                            }
                            else
                            {
                                currentVariableDictionary.Add(variable, localStack.Pop());
                            }
                        } 
                        break;
                    case ("rvalue"):
                        int value;
                        if (inBegin)
                        {
                            if (currentVariableDictionary.TryGetValue(line[1], out value)) { }
                            else if (nextVariableDictionary.TryGetValue(line[1], out value)) { }
                        }
                        else if (inMethod)
                        {
                            currentVariableDictionary.TryGetValue(line[1], out value);
                        }
                        else
                        {
                            currentVariableDictionary.TryGetValue(line[1], out value);
                        }
                        localStack.Push(value);
                        break;
                    case ("print"):
                        Console.WriteLine(localStack.Peek());
                         break;
                    case ("pop"):
                        localStack.Pop();
                        break;
                    case ("copy"):
                        localStack.Push(localStack.Peek());
                        break;
                    case ("<>"):
                        if (localStack.Pop() == localStack.Pop())
                        {
                            localStack.Push(0);
                        }
                        else
                        {
                            localStack.Push(1);
                        }
                        break;
                    case ("<="):
                        if (localStack.Pop() >= localStack.Pop())
                        {
                            localStack.Push(1);
                        }
                        else
                        {
                            localStack.Push(0);
                        }
                        break;
                    case (">="):
                        if (localStack.Pop() <= localStack.Pop())
                        {
                            localStack.Push(1);
                        }
                        else
                        {
                            localStack.Push(0);
                        }
                        break;
                    case ("<"):
                        if (localStack.Pop() > localStack.Pop())
                        {
                            localStack.Push(1);
                        }
                        else
                        {
                            localStack.Push(0);
                        }
                        break;
                    case (">"):
                        if (localStack.Pop() < localStack.Pop())
                        {
                            localStack.Push(1);
                        }
                        else
                        {
                            localStack.Push(0);
                        }
                        break;
                    case ("="):
                        if (localStack.Pop() == localStack.Pop())
                        {
                            localStack.Push(1);
                        }
                        else
                        {
                            localStack.Push(0);
                        }
                        break;
                    case ("+"):
                        localStack.Push(localStack.Pop() + localStack.Pop());
                        break;
                    case ("-"):
                        temp = localStack.Pop();
                        localStack.Push(localStack.Pop() - temp);
                        break;
                    case ("/"):
                        temp = localStack.Pop();
                        localStack.Push(localStack.Pop() / temp);
                        break;
                    case ("*"):
                        localStack.Push(localStack.Pop() * localStack.Pop());
                        break;
                    case ("div"):
                        temp = localStack.Pop();
                        localStack.Push(localStack.Pop() % temp);
                        break;
                    case ("&"):
                        localStack.Push(localStack.Pop() & localStack.Pop());
                        break;
                    case ("|"):
                        localStack.Push(localStack.Pop() | localStack.Pop());
                        break;
                    case ("!"):
                        localStack.Push(~localStack.Pop());
                        break;
                    case ("begin"):
                        inBegin = true;
                        for(int k = i; k < lineDictionary.Count; k++)
                        {
                            string[] tempLine = lineDictionary[k]; 
                            if(tempLine[0] == "call")
                            {
                                if(tempLine[1] == lastCall)
                                {
                                    inRecursion = true;
                                }
                            }
                        }
                        break;
                    case ("end"):
                        inBegin = false;
                        nextVariableDictionary.Clear();
                        break;
                    case ("call"):
                        variableStackFrame.Push(currentVariableDictionary);
                        currentVariableDictionary = nextVariableDictionary;
                        nextVariableDictionary = new Dictionary<string, int>();

                        inBegin = false;
                        inMethod = true;

                        i += 1;
                        returnAddressStack.Push(i);

                        i = labelsDict[line[1]];
                        //avoids jumping over label
                        i -= 1;
                        lastCall = line[1];
                        break;
                    case ("return"):
                        nextVariableDictionary = currentVariableDictionary;
                        currentVariableDictionary = variableStackFrame.Pop();
                        inMethod = false;
                        inBegin = true;
                        i = returnAddressStack.Pop();
                        //i is incremented at the end, so this stops it from
                        //going one too far on where it's returning to
                        i -= 1;
                        break;
                    case ("goto"):
                        label = line[1];
                        i = labelsDict[label];
                        break;
                    case ("gofalse"):
                        if(localStack.Pop() == 0)
                        {
                            label = line[1];
                            i = labelsDict[label];
                        }
                        break;
                    case ("gotrue"):
                        if (localStack.Pop() != 0)
                        {
                            label = line[1];
                            i = labelsDict[label];
                        }
                        break;
                    default:
                        break;
                }
                if (label.Equals(""))
                {
                    i++;
                }
            }
        }
    }
}
