using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    public class ParamReader
    {
        public static List<string> KeyWords { get; set; } = new List<string>() { "Execute", "Skip" };
        public static string PARAMS_PATTERN = @"^\s*(\s*[a-zA-Z]+\s*=\s*[a-zA-Z0-9]+((,\s*[<>]\s?[0-9]+)(,\s*(([+]{2})|([-]{2})|([-+][=]\s?[0-9]+))))?;)\s*$";
        public static string PARAM_EXTRACT_PATTERN = @"[a-zA-Z]+\s*=\s*[a-zA-Z0-9]+((,\s*[<>]\s?[0-9]+)(,\s*(([+]{2})|([-]{2})|([-+][=]\s?[0-9]+))))?";
        public static string OPERATORS_PATTERN = @"(([+]{2})|([-]{2})|([+-][=])|([<>]))";
        public static string DIGIT_PATTERN = @"[0-9]+";
        //Methods
        public KeyValuePair<bool,string> ReturnResult(string _params,string _condition)
        {
            var args = ParseParamsString(_params);
            var cond = ParseString(_condition);
            KeyValuePair<bool, string> result = new KeyValuePair<bool, string>(false, "Execute");
            foreach (var condition in cond)
            {
                var arg = args.Find(x => x.Param == condition.Param);
                if (arg == null)
                    break; 
                switch (condition.Operator)
                {
                    case "==":
                        if (arg.Value == condition.Value)
                            result = new KeyValuePair<bool, string>(true, condition.Action);
                        break;
                    case "!=":
                        if (arg.Value != condition.Value)
                            result = new KeyValuePair<bool, string>(true, condition.Action);
                        break;
                }
                if (result.Key == false)
                    break;
            }
            return result;
        }
        public List<ParamsArgs> ParseString(string str)
        {
            List<ParamsArgs> args = new List<ParamsArgs>();
            var sections = str.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries);
            foreach (var section in sections)
            {
                var result = section.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                args.Add(new ParamsArgs(result[0], result[1], result[2]));
                if (result.Count() == 4)
                    args.Last().Action = result[3];
            }
            return args;
        }
        public List<ParamsArgs> ParseParamsString(string str)
        {
            List<ParamsArgs> args = new List<ParamsArgs>();
            //var sections = Regex.Matches(str, PARAM_EXTRACT_PATTERN);
            var sections = str.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in sections)
            {
                var subsections = item.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries);
                var param = subsections[0].Split(new char[] { '=',' '},StringSplitOptions.RemoveEmptyEntries);
                args.Add(new ParamsArgs());
                args.Last().ParamName = param[0];
                args.Last().ParamName = param[1];
                if (subsections.Length > 1)
                {
                    var _conditionOperator = Regex.Matches(subsections[1], OPERATORS_PATTERN);
                    var _conditionValue = Regex.Matches(subsections[1], DIGIT_PATTERN);
                    var _postOperator = Regex.Matches(subsections[2], OPERATORS_PATTERN);
                    var _postValue = Regex.Matches(subsections[2], DIGIT_PATTERN);

                    args.Last().ConditionOperator = _conditionOperator.Count > 0 ? _conditionOperator[0].Value : "none";
                    args.Last().ConditionValue = _conditionValue.Count > 0 ? _conditionValue[0].Value : "none"; ;
                    args.Last().PostOperator = _postOperator.Count > 0 ? _postOperator[0].Value : "none";
                    args.Last().PostValue = _postValue.Count > 0 ? _postValue[0].Value : "none"; ;
                }
            }
            return args;
        }
    }

    public class ParamsArgs
    {
        //Properties
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
        public string ConditionOperator { get; set; }
        public string ConditionValue { get; set; }
        public string PostOperator { get; set; }
        public string PostValue { get; set; }
        //Constructor
        public ParamsArgs()
        {
           
        }
    }
    public class ConditionArgs
    {
        //Properties
        public string Param { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        //Constructor
        public ConditionArgs(string _param, string _operator, string _value)
        {
            Param = _param;
            Operator = _operator;
            Value = _value;
            Action = "Execute";
        }
        public ConditionArgs(string _param, string _operator, string _value, string _action) : this(_param, _operator, _value)
        {
            Action = _action;
        }
    }
}
