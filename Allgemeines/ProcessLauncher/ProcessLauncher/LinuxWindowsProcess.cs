using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ProcessLauncher
{
    public class LinuxWindowsProcess
    {
        public StringBuilder Test(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // What is going to do Mono with this value?
            startInfo.CreateNoWindow = true;

            // Set UseShellExecute to false for redirection.
            // To use StandardOutput, you must set ProcessStartInfo.UseShellExecute to false, and you must set
            // ProcessStartInfo.RedirectStandardOutput to true. Otherwise, reading from the StandardOutput stream throws an exception.
            // What is going to do Mono with stdout, stdin and stderr depending on this value?
            startInfo.UseShellExecute = false;

            // Redirect the standard output of the subprocess command.   
            // What is going to do Mono with stdout? Some pipe, I guess.
            startInfo.RedirectStandardOutput = true;

            // What is going to do Mono with stdin?
            startInfo.RedirectStandardInput = false;

            // What is going to do Mono with stderr?
            startInfo.RedirectStandardError = false;

            // What is going to do Mono with this value? What if I am running a GTK (GUI application)
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            startInfo.Arguments = arguments;
            startInfo.FileName = command;




            // Define variables shared by class methods. 
            StringBuilder processOutput = new StringBuilder();

            Process process = new Process();

            process.StartInfo = startInfo;

            // When is Mono going to call my event implementation? Every time the subprocess writes to stdout?
            // From here: http://msdn.microsoft.com/en-us/library/vstudio/system.diagnostics.process.outputdatareceived%28v=vs.110%29.aspx
            // Thereafter, the OutputDataReceived event signals each time the process writes a line to the
            // redirected StandardOutput stream, until the process exits or calls CancelOutputRead.
            // That behaviour is weird (IMHO) Check out the Mono implementation!!!
            // Set our event handler to asynchronously read the standard output.
            process.OutputDataReceived += new DataReceivedEventHandler(
            delegate(object sendingProcess, DataReceivedEventArgs outLine)
            {
                // Collect the command output.
                // To asynchronously collect the redirected StandardOutput or StandardError stream output of a process, you must create
                // a method that handles the redirected stream output events. The event-handler method is called when the process writes
                // to the redirected stream. The event delegate calls your event handler with an instance of DataReceivedEventArgs.
                // The Data property contains the text line that the process wrote to the redirected stream.
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    // Add the text to the collected output.
                    processOutput.Append(Environment.NewLine + outLine.Data);
                }
            });
            // Taken from http://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net
            // I do not think this is the right way to read the output stream!!!!
            // My event handler will be called (see documentation) every time the process writes a line to the output stream but
            // with this code I will be reading the output stream until it is closed by the process. So, I guess, we could end
            // up having multiple event handlers reading from the same stream (output stream) Each handler was called each time
            // the process wrote a line to the output stream. :/
            // process.OutputDataReceived += new DataReceivedEventHandler(
            // delegate(object sender, DataReceivedEventArgs e)
            // {                   
            //     using (StreamReader output = process.StandardOutput)
            //    {
            //        string data = output.ReadToEnd();
            //        processOutput.Append(data);
            //    }
            // });
            // Don't mix using statements and lambda expressions :(
            // http://blogs.msdn.com/b/jaredpar/archive/2008/07/16/don-t-mix-using-statements-and-lambda-expressions.aspx
            // process.OutputDataReceived += (sender, e) => 
            // {
            //    using (StreamReader output = process.StandardOutput)
            //    {
            //        string data = output.ReadToEnd();
            //        processOutput.Append(data);
            //    }
            // };

            // Start the process.
            process.Start();

            // Start the asynchronous read of the output stream.
            process.BeginOutputReadLine();

            // Wait for the process.
            process.WaitForExit();

            process.Close();

            return processOutput;
        }
    }
}

