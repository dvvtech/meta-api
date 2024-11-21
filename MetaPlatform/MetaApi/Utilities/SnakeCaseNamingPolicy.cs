using System.Text.Json;

namespace MetaApi.Utilities
{
    /// <summary>
    /// названия полей с маленькой буквы и если несколько слов то объединяются через разделитель _
    /// </summary>
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var buffer = new System.Text.StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    if (i > 0)
                        buffer.Append('_');
                    buffer.Append(char.ToLower(name[i]));
                }
                else
                {
                    buffer.Append(name[i]);
                }
            }
            return buffer.ToString();
        }
    }
}
