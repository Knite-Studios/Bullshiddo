namespace Oculus.Interaction.Bullshiddo
{
    public class StringPropertyBehaviour : PropertyBehaviour<string>
    {
        public string startValue = "";

        private void Awake()
        {
            if (string.IsNullOrEmpty(Value))
            {
                Value = startValue;
            }
        }
    }
}
