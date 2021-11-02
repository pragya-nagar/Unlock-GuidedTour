using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


namespace GuidedTour.EF.Entity
{
    [ExcludeFromCodeCoverage]
    public class EntityBase : IObjectState
    {
        [NotMapped]
        public ObjectState ObjectStateEnum { get; set; }
    }
}
