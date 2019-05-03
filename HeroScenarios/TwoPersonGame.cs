namespace HeroScenarios
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TwoPersonGame
    {
        public TwoPersonGame()
        {
        }

        public TwoPersonGame(string day, string id)
        {
            this.Day = day;
            this.Id = id;
        }

        public string Id { get; set; }

        public string Day { get; set; }

        public uint User1Score { get; set; }

        public uint User2Score { get; set; }
    }
}
