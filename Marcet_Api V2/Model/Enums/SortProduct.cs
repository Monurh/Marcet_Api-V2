using System.Runtime.Serialization;

namespace Marcet_Api_V2.Model.Enums
{
    public enum SortProduct
    {
        [EnumMember(Value = "А-Я")]
        NameAsc, // по Алфовиту
        [EnumMember(Value = "Я-А")]
        NameDesc, // не по алфовиту
        [EnumMember(Value = "За зростанням ")]
        PriceAsc, // повазростанию цены
        [EnumMember(Value = "За спаданням")]
        PriceDesc, // по убыванию цены
        [EnumMember(Value = "За категоріями")]
        Category
    }
}
