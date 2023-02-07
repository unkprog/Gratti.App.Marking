using System.ComponentModel.DataAnnotations;

namespace Gratti.App.Marking.Api
{
    public enum GroupEnum
    {
        [Display(Name = Constants.group_lp)]
        lp = 1,
        [Display(Name = Constants.group_shoes)]
        shoe = 2,
        [Display(Name = Constants.group_perfum)]
        perfum = 4,
        [Display(Name = Constants.group_tires)]
        tires = 5,
        [Display(Name = Constants.group_photo)]
        photo = 6,
        [Display(Name = Constants.group_milk)]
        milk = 8,
        [Display(Name = Constants.group_water)]
        water = 13,
        [Display(Name = Constants.group_beer)]
        beer = 15
    }
}
