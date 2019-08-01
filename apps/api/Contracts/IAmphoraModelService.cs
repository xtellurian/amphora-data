using System;
using System.Collections.Generic;
using api.Models;

namespace api.Contracts
{
    public interface IAmphoraModelService
    {
        IEnumerable<string> ListAmphoraeIds();
        AmphoraModel GetAmphora(string id);
        AmphoraModel SetAmphora(AmphoraModel model);
    }
}