namespace Eze.Quantbox
{
    public class EmsSettings
    {
        public string Gateway { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string Customer { get; set; }
        public string Deposit { get; set; }

        public static EmsSettings CreateDefault()
        {
            return new EmsSettings()
            {
                Gateway = "stgtsperf1.dev.local",
                Bank = "VALENTINE",
                Branch = "WINTHROPE",
                Customer = "MORTIMER",
                Deposit = "NEUTRAL"
            };
        }
    }
}