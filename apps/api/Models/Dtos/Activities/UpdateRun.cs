namespace Amphora.Api.Models.Dtos.Activities
{
    public class UpdateRun
    {
        public UpdateRun() { }

        public UpdateRun(bool? success)
        {
            Success = success;
        }

        /// <summary>
        /// Gets or sets whether the run failed or succeeded. Setting this will end the run.
        /// </summary>
        public bool? Success { get; set; }
    }
}