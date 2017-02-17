﻿using UnityEngine;
using System.Collections.Generic;
using System;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public class LuaNode : FlowNode
    {
        protected const string kBeginConfigValueInput = "--BEGIN_VALUE_INPUT_CONFIG--";
        protected const string kEndConfigValueInput = "--END_VALUE_INPUT_CONFIG--";

        protected const string kBeginConfigValueOutput = "--BEGIN_VALUE_OUTPUT_CONFIG--";
        protected const string kEndConfigValueOutput = "--END_VALUE_OUTPUT_CONFIG--";

        protected const string kBeginConfigFlowInput = "--BEGIN_FLOW_INPUT_CONFIG--";
        protected const string kEndConfigFlowInput = "--END_FLOW_INPUT_CONFIG--";

        protected const string kBeginConfigFlowOutput = "--BEGIN_FLOW_OUTPUT_CONFIG--";
        protected const string kEndConfigFlowOutput = "--END_FLOW_OUTPUT_CONFIG--";

        protected string _luaFilePath = "";
        protected string _luaFileName = "";
        
        protected List<ValueInput> _autoValueInputs = new List<ValueInput>();
        protected List<string> _autoValueInputArgNames = new List<string>();

        protected List<FlowOutput> _autoFlowOuts = new List<FlowOutput>();

        public bool ParseHeadConfig(string context, string beginConfig, string endConfig,
           out Dictionary<string, string> resDic)
        {
            resDic = new Dictionary<string, string>();
            int beginIndex = context.IndexOf(beginConfig);
            int endIndex = context.IndexOf(endConfig);

            if (beginIndex < 0 || endIndex < 0)
            {
                return true;
            }

            string configContext = context.Substring(beginIndex, endIndex - beginIndex);
            beginIndex = configContext.IndexOf("{") + 1;
            endIndex = configContext.IndexOf("}") - 1;

            if (beginIndex < 0 || endIndex < 0)
            {
                return false;
            }

            string argsContext = configContext.Substring(beginIndex, endIndex - beginIndex);
            string[] args = argsContext.Split(',');
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                string[] keyValue = arg.Split('=');
                string argName = keyValue[0].Trim();
                string argType = keyValue[1].Trim();
                argType = argType.Replace("\"", "");
                Debug.Log(argName + "   " + argType);
                resDic.Add(argName, argType);
            }

            return true;
        }

        virtual public void Config(string luaFilePath)
        {
            _luaFilePath = luaFilePath;
            _luaFileName = System.IO.Path.GetFileNameWithoutExtension(_luaFilePath);
        }

        protected override void RegisterPorts()
        {
            AutoGeneratePort();
        }

        protected void AutoGeneratePort()
        {
            if (!string.IsNullOrEmpty(_luaFilePath))
            {
                string fileContext = System.IO.File.ReadAllText(_luaFilePath);
                AutoGenerateValueInput(fileContext);
                AutoGenerateValueOutput(fileContext);
                AutoGenerateFlowInput(fileContext);
                AutoGenerateFlowOutput(fileContext);
            }

        }

        protected virtual void AutoGenerateValueInput(string fileContext)
        {
            Dictionary<string, string> valueInputconf;
            if (!ParseHeadConfig(fileContext, kBeginConfigValueInput, kEndConfigValueInput, out valueInputconf))
            {
                Debug.LogError(string.Format("{0} : {1}, {2} error ", _luaFilePath, kBeginConfigValueInput,
                    kEndConfigValueInput));
            }
            foreach (KeyValuePair<string, string> conf in valueInputconf)
            {
                string argName = conf.Key;
                string argType = conf.Value;
                Type t = Type.GetType(argType);
                var portType = typeof(ValueInput<>).RTMakeGenericType(new Type[] { t });
                var port = (ValueInput)Activator.CreateInstance(portType, new object[] { this, argName, argName });
                AddValueInput(port, argName);
                _autoValueInputs.Add(port);
                _autoValueInputArgNames.Add(argName);
            }
        }

        protected virtual void AutoGenerateValueOutput(string fileContext)
        {
            //Dictionary<string, string> valueOutputconf;
            //if (!ParseHeadConfig(fileContext, kBeginConfigValueOutput, kEndConfigValueOutput, out valueOutputconf))
            //{
            //    Debug.LogError(string.Format("{0} : {1}, {2} error ", _luaFilePath, kBeginConfigValueOutput,
            //        kEndConfigValueOutput));
            //}

            //foreach (KeyValuePair<string, string> conf in valueOutputconf)
            //{
            //    string argName = conf.Key;
            //    string argType = conf.Value;
            //    Type t = Type.GetType(argType);
            //    var portType = typeof(ValueOutput<>).RTMakeGenericType(new Type[] { t });
            //    var port = (ValueOutput)Activator.CreateInstance(portType, new object[] { this, argName, argName });
            //}

        }

        protected virtual void AutoGenerateFlowInput(string fileContext) {
            //Dictionary<string, string> flowInputconf;
            //if (!ParseHeadConfig(fileContext, kBeginConfigFlowInput, kEndConfigFlowInput, out flowInputconf))
            //{
            //    Debug.LogError(string.Format("{0} : {1}, {2} error ", _luaFilePath, kBeginConfigFlowInput,
            //        kEndConfigFlowInput));
            //}


            //foreach (KeyValuePair<string, string> conf in flowInputconf)
            //{
            //    string argName = conf.Key;
            //    FlowInput flowInput = AddFlowInput(argName, (Flow f) =>{});
            //    _autoFlowIns.Add(flowInput);
            //}

        }
        protected virtual void AutoGenerateFlowOutput(string fileContext)
        {
            Dictionary<string, string> flowOutputconf;
            if (!ParseHeadConfig(fileContext, kBeginConfigFlowOutput, kEndConfigFlowOutput, out flowOutputconf))
            {
                Debug.LogError(string.Format("{0} : {1}, {2} error ", _luaFilePath, kBeginConfigFlowOutput,
                    kEndConfigFlowOutput));
            }

            foreach (KeyValuePair<string, string> conf in flowOutputconf)
            {
                string argName = conf.Key;
                FlowOutput flowOut = AddFlowOutput(argName);
                _autoFlowOuts.Add(flowOut);
            }
        }
    }
}
