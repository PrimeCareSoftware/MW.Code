namespace MedicSoft.Domain.ValueObjects
{
    public class PhysicalExamination
    {
        public string? GeneralAppearance { get; private set; }
        public string? Head { get; private set; }
        public string? Eyes { get; private set; }
        public string? Ears { get; private set; }
        public string? Nose { get; private set; }
        public string? Throat { get; private set; }
        public string? Neck { get; private set; }
        public string? Cardiovascular { get; private set; }
        public string? Respiratory { get; private set; }
        public string? Abdomen { get; private set; }
        public string? Musculoskeletal { get; private set; }
        public string? Neurological { get; private set; }
        public string? Skin { get; private set; }
        public string? OtherFindings { get; private set; }

        private PhysicalExamination() { }

        public PhysicalExamination(
            string? generalAppearance = null, string? head = null, 
            string? eyes = null, string? ears = null,
            string? nose = null, string? throat = null, 
            string? neck = null, string? cardiovascular = null,
            string? respiratory = null, string? abdomen = null, 
            string? musculoskeletal = null, string? neurological = null,
            string? skin = null, string? otherFindings = null)
        {
            GeneralAppearance = generalAppearance?.Trim();
            Head = head?.Trim();
            Eyes = eyes?.Trim();
            Ears = ears?.Trim();
            Nose = nose?.Trim();
            Throat = throat?.Trim();
            Neck = neck?.Trim();
            Cardiovascular = cardiovascular?.Trim();
            Respiratory = respiratory?.Trim();
            Abdomen = abdomen?.Trim();
            Musculoskeletal = musculoskeletal?.Trim();
            Neurological = neurological?.Trim();
            Skin = skin?.Trim();
            OtherFindings = otherFindings?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not PhysicalExamination other)
                return false;

            return GeneralAppearance == other.GeneralAppearance &&
                   Head == other.Head &&
                   Eyes == other.Eyes &&
                   Ears == other.Ears &&
                   Nose == other.Nose &&
                   Throat == other.Throat &&
                   Neck == other.Neck &&
                   Cardiovascular == other.Cardiovascular &&
                   Respiratory == other.Respiratory &&
                   Abdomen == other.Abdomen &&
                   Musculoskeletal == other.Musculoskeletal &&
                   Neurological == other.Neurological &&
                   Skin == other.Skin &&
                   OtherFindings == other.OtherFindings;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(GeneralAppearance);
            hash.Add(Head);
            hash.Add(Eyes);
            hash.Add(Ears);
            hash.Add(Nose);
            hash.Add(Throat);
            hash.Add(Neck);
            hash.Add(Cardiovascular);
            hash.Add(Respiratory);
            hash.Add(Abdomen);
            hash.Add(Musculoskeletal);
            hash.Add(Neurological);
            hash.Add(Skin);
            hash.Add(OtherFindings);
            return hash.ToHashCode();
        }
    }
}
