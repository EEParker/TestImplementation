﻿namespace Sttp.Codec.Metadata
{
    public class CmdFinished : ICmd
    {
        public SubCommand SubCommand => SubCommand.Finished;

        public void Load(PayloadReader reader)
        {
        }

    }
}