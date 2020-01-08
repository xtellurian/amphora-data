using System;
using System.Collections;
using System.Collections.Generic;

namespace Amphora.Common.Models.Amphorae
{
    public class PinnedAmphorae : IEnumerable<AmphoraModel>
    {
        public string? AmphoraId1 { get; set; }
        public virtual AmphoraModel? Amphora1 { get; set; }
        public string? AmphoraId2 { get; set; }
        public virtual AmphoraModel? Amphora2 { get; set; }
        public string? AmphoraId3 { get; set; }
        public virtual AmphoraModel? Amphora3 { get; set; }
        public string? AmphoraId4 { get; set; }
        public virtual AmphoraModel? Amphora4 { get; set; }
        public string? AmphoraId5 { get; set; }
        public virtual AmphoraModel? Amphora5 { get; set; }
        public string? AmphoraId6 { get; set; }
        public virtual AmphoraModel? Amphora6 { get; set; }

        public bool AreAllNull()
        {
            return AmphoraId1 == null &&
                AmphoraId2 == null &&
                AmphoraId3 == null &&
                AmphoraId4 == null &&
                AmphoraId5 == null &&
                AmphoraId6 == null;
        }

        public bool AreAnyNull()
        {
            return Contains(null);
        }

        public bool IsPinned(AmphoraModel amphora)
        {
            return this.Contains(amphora.Id);
        }

        private bool Contains(string? id)
        {
            return AmphoraId1 == id ||
                AmphoraId2 == id ||
                AmphoraId3 == id ||
                AmphoraId4 == id ||
                AmphoraId5 == id ||
                AmphoraId6 == id;
        }

        public IEnumerator<AmphoraModel> GetEnumerator()
        {
            if (Amphora1 != null) { yield return Amphora1; }
            if (Amphora2 != null) { yield return Amphora2; }
            if (Amphora3 != null) { yield return Amphora3; }
            if (Amphora4 != null) { yield return Amphora4; }
            if (Amphora5 != null) { yield return Amphora5; }
            if (Amphora6 != null) { yield return Amphora6; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Unpin(AmphoraModel amphora)
        {
            var id = amphora.Id;
            if (AreAllNull()) { return; }
            if (IsPinned(amphora))
            {
                if (AmphoraId1 == id)
                {
                    AmphoraId1 = null;
                    Amphora1 = null;
                }
                else if (AmphoraId2 == id)
                {
                    AmphoraId2 = null;
                    Amphora2 = null;
                }
                else if (AmphoraId3 == id)
                {
                    AmphoraId3 = null;
                    Amphora3 = null;
                }
                else if (AmphoraId4 == id)
                {
                    AmphoraId4 = null;
                    Amphora4 = null;
                }
                else if (AmphoraId5 == id)
                {
                    AmphoraId5 = null;
                    Amphora5 = null;
                }
                else if (AmphoraId6 == id)
                {
                    AmphoraId6 = null;
                    Amphora6 = null;
                }
            }
        }

        public void PinToLeastNull(AmphoraModel amphora)
        {
            if (AreAnyNull())
            {
                if (IsPinned(amphora)) { return; }
                if (AmphoraId1 == null)
                {
                    AmphoraId1 = amphora.Id;
                    Amphora1 = amphora;
                }
                else if (AmphoraId2 == null)
                {
                    AmphoraId2 = amphora.Id;
                    Amphora2 = amphora;
                }
                else if (AmphoraId3 == null)
                {
                    AmphoraId3 = amphora.Id;
                    Amphora3 = amphora;
                }
                else if (AmphoraId4 == null)
                {
                    AmphoraId4 = amphora.Id;
                    Amphora4 = amphora;
                }
                else if (AmphoraId5 == null)
                {
                    AmphoraId5 = amphora.Id;
                    Amphora5 = amphora;
                }
                else if (AmphoraId6 == null)
                {
                    AmphoraId6 = amphora.Id;
                    Amphora6 = amphora;
                }
            }
            else
            {
                throw new ArgumentException("No free spaces to pin");
            }
        }
    }
}