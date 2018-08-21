
namespace Macro_Commander.src
{
    public class Logger
    {
        //Fields
        private static Logger _logger;

        //Constructor
        private Logger()
        {
            using (System.IO.StreamWriter stream = new System.IO.StreamWriter("log.txt",true))
            {
                stream.WriteLine($"{System.DateTime.Now} - LoggerCreated");
            }
        }

        //Methods
        public static Logger GetLogger()
        {
            return _logger ?? (_logger = new Logger());
        }
        public void WriteToLog(string message)
        {
            using (System.IO.StreamWriter stream = new System.IO.StreamWriter("log.txt",true))
            {
                stream.WriteLine($"\t {System.DateTime.Now.ToLongTimeString()} - {message}");
            }
        }
        public void CatchException(string className,string method,string exMessage)
        {
            WriteToLog($"{className} : {method} : Exception{{{exMessage}}}");
        }
    }
}
