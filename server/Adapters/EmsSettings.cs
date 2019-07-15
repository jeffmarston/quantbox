namespace Eze.Quantbox
{
    public class EmsSettings
    {
        public string Gateway { get; internal set; }
        public string Bank { get; internal set; }
        public string Branch { get; internal set; }
        public string Customer { get; internal set; }
        public string Deposit { get; internal set; }

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