using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PgpSharp.GnuPG
{
    [TestClass]
    public class RedirectedProcessTests
    {
        [TestMethod]
        public void Output_Timing_Test()
        {
            // may fail if implementation is broken
            // see http://alabaxblog.info/2013/06/redirectstandardoutput-beginoutputreadline-pattern-broken/

            int timeout = 1000;
            int runs = 500;


            for (int i = 0; i < runs; ++i)
            {
                string testString = Guid.NewGuid().ToString();

                using (var proc = new RedirectedProcess("cmd.exe", string.Format("/c echo {0}", testString)))
                {
                    if (proc.Start())
                    {
                        proc.WaitForExit(timeout);

                        Assert.AreEqual(0, proc.ExitCode, "Wrong exit code.");

                        Assert.AreEqual(testString, proc.Output.TrimEnd(), "Wrong output."); // may fail here

                        Assert.AreEqual(string.Empty, proc.Error.TrimEnd(), "Wrong error.");
                    }
                }
            }
        }
    }
}
