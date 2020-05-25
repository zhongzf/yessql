using System;
using System.Collections.Generic;
using System.Text;

namespace YesSql.Provider.Cosmos.Helpers
{
    public class CommandConverter
    {
        private string commandText;

        public CommandConverter(string commandText)
        {
            this.commandText = commandText;
        }

        public string Convert()
        {
            if(commandText == "SELECT nextval FROM Identifiers WHERE dimension = @dimension;")
            {
                return "SELECT Identifiers.nextval FROM Identifiers WHERE Identifiers.dimension = @dimension";
            }
            else if(commandText == "SELECT Document.* FROM Document WHERE Document.Type = @Type OFFSET 0 LIMIT 1")
            {
                return "SELECT * FROM Document WHERE Document.Type = @Type OFFSET 0 LIMIT 1";
            }
            return commandText;
        }
    }
}
