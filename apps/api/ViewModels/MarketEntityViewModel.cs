using System.ComponentModel.DataAnnotations;
using Amphora.Common.Models;

namespace Amphora.Api.ViewModels
{
    public abstract class MarketEntityViewModel : Entity
    {
        
        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

    }
}