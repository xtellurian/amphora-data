namespace Amphora.SharedUI.Models
{
    public class EnvironmentModel
    {
        public EnvironmentModel(string environmentName)
        {
            this.EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; }
    }
}