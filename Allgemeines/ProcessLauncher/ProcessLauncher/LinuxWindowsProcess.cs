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
            // Answer: true or false is the same (from the stdout/stderr/stdin point of view) BUT IF THIS PROPERTY
            // HAS TRUE VALUE YOU MAY NOT REDIRECT stdout/stderr/stdin STREAMS. SO, THIS PROPERTY
            // ENABLES US TO REDIRECT STREAMS (by means of pipes). WHEN UseShellExecute HAS TRUE VALUE TRYING
            // TO SET TRUE VALUE IN RedirectStandardOutput/RedirectStandardInput/RedirectStandardError
            // WILL THROW EXCEPTION.
            startInfo.UseShellExecute = false;

            // Redirect the standard output of the child process command.
            // What is going to do Mono with stdout? Answer: piped stdout from child to this C# parent process.
            // TRUE VALUE REQUIRES UseShellExecute=false
            startInfo.RedirectStandardOutput = true;

            // What is going to do Mono with stdin? Answer: stdin the same as child's parent process (this C# process)
            // TRUE VALUE REQUIRES UseShellExecute=false
            startInfo.RedirectStandardInput = false;

            // What is going to do Mono with stderr? Answer: stderr the same as child's parent process (this C# process)
            // TRUE VALUE REQUIRES UseShellExecute=false
            startInfo.RedirectStandardError = false;

            // What is going to do Mono with this value? What if I am running a GTK (GUI application)
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            // Encoding
            startInfo.StandardOutputEncoding = Encoding.UTF8;
            // It is only supported when stream is redirected.
            // startInfo.StandardErrorEncoding = Encoding.UTF8;

            startInfo.Arguments = arguments;
            startInfo.FileName = command;




            // Define variables shared by class methods. 
            StringBuilder processOutput = new StringBuilder();

            Process process = new Process();

            process.StartInfo = startInfo;

            // What is going to do Mono depending on this value? :/
            // true if the Exited event should be raised when the associated child process is terminated
            // (through either an exit or a call to Kill); otherwise, false. The default is false.
            // The EnableRaisingEvents property indicates whether the component should be notified
            // when the operating system has shut down a child process. The EnableRaisingEvents property
            // is used in asynchronous processing to notify your application that a child process has exited.
            // To force your application to synchronously wait for an exit event (which interrupts
            // processing of the application until the exit event has occurred), use the WaitForExit method.
            process.EnableRaisingEvents = false;

            // When is Mono going to call my event implementation? Every time the child process writes to stdout?
            // From here: http://msdn.microsoft.com/en-us/library/vstudio/system.diagnostics.process.outputdatareceived%28v=vs.110%29.aspx
            // Thereafter, the OutputDataReceived event signals each time the child process writes a line to the
            // redirected StandardOutput stream, until the child process exits or calls CancelOutputRead.
            // That behaviour is weird (IMHO) Check out the Mono implementation!!!
            // Set our event handler to asynchronously read the standard output.
            process.OutputDataReceived += new DataReceivedEventHandler(
            delegate(object sendingProcess, DataReceivedEventArgs outLine)
            {
                // Collect the command output.
                // To asynchronously collect the redirected StandardOutput or StandardError stream output of a child process, you must create
                // a method that handles the redirected stream output events. The event-handler method is called when the child process writes
                // to the redirected stream. The event delegate calls your event handler with an instance of DataReceivedEventArgs.
                // The Data property contains the text line that the child process wrote to the redirected stream.
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    // Add the text to the collected output.
                    processOutput.Append(Environment.NewLine + outLine.Data);
                }
            });

            // Start the process.
            process.Start();

            // Start the asynchronous read of the output stream.
            // What is going to do Mono with this method? I guess, it will begin to read from the piped stream
            // line by line and it will call to my DataReceivedEventHandler delegate implementation for every line
            // (by means of some new thread in an asynchronous way)
            process.BeginOutputReadLine();

            // BE CAREFUL WITH DEADLOCKS (if child process fills pipes and nobody reads them, this C# parent process
            // could wait forever because the child process would be waiting until someone reads from the pipes)
            // In this case, the process, which should read from the stdout pipe, is this C# parent process.
            // see: http://msdn.microsoft.com/en-us/library/vstudio/system.diagnostics.process.standardoutput

            // Wait for the process.
            process.WaitForExit();

            process.Close();

            return processOutput;
        }
    }
}

