using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.ImranStuff.Scripts
{
    public class AITransactionHandler
    {

        public void SendDataToCloud(MemoryStream WAVdata)
        {
            string FileName = @"c:\temp\GvrTest.wav";
            WAVdata.Position = 0;
            FileStream FSWriteMe;
            FSWriteMe = new FileStream(FileName, FileMode.Create);
            FSWriteMe.Write(WAVdata.ToArray(), 0, (int)WAVdata.Length);
            FSWriteMe.Flush();
            FSWriteMe.Dispose();
            return;
        }


    }
}
