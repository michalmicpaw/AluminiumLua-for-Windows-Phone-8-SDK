/*
LuaContext.cs: Represents a scope
Copyright (c) 2011 Alexander Corrado
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


using System;
using System.Collections.Generic;

namespace AluminumLua
{

    public partial class LuaContext
    {
        // (see libraries)

        private IDictionary<string, LuaObject> variables;
        public IDictionary<string, LuaObject> Variables
        {
            get
            {
                if (variables == null)
                    variables = new Dictionary<string, LuaObject>();

                return variables;
            }
        }

        protected LuaContext Parent { get; private set; }

        public LuaContext(LuaContext parent)
        {
            this.Parent = parent;

            if (Parent.variables != null)
                this.variables = new LinkedDictionary<string, LuaObject>(parent.variables);
        }

        public LuaContext()
        {
            SetGlobal("true", LuaObject.True);
            SetGlobal("false", LuaObject.False);
            SetGlobal("nil", LuaObject.Nil);
        }

        public LuaObject Get(string name)
        {
            LuaObject val;
            Variables.TryGetValue(name, out val);
            return val;
        }

        public bool IsDefined(string name)
        {
            return Variables.ContainsKey(name);
        }

        public void Define(string name)
        {
            if (!IsDefined(name))
                Variables.Add(name, LuaObject.Nil);
        }

        public void SetLocal(string name, LuaObject val)
        {
            if (Variables.ContainsKey(name))
                Variables.Remove(name);

            if (!val.IsNil)
                Variables.Add(name, val);
        }
        public void SetLocal(string name, LuaFunction fn)
        {
            SetLocal(name, LuaObject.FromFunction(fn));
        }

        public void SetGlobal(string name, LuaObject val)
        {
            SetLocal(name, val);
            if (Parent != null)
                Parent.SetGlobal(name, val);
        }
        public void SetGlobal(string name, LuaFunction fn)
        {
            SetGlobal(name, LuaObject.FromFunction(fn));
        }


        public void SetLocalAndParent(string name, LuaObject val)
        {
            SetLocal(name, val);
            Parent.SetLocal(name, val);
        }
        public void SetLocalAndParent(string name, LuaFunction fn)
        {
            SetLocalAndParent(name, LuaObject.FromFunction(fn));
        }
    }
}