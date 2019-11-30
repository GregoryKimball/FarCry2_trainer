# FarCry2_trainer

WPF application for creating Far Cry 2 mods.  

The tools are based on the great work by Gibbed, and the necessary binaries are included under ./Resources/gibbed.zip.  

Dependencies under https://github.com/gibbed/
Gibbed.Dunia
Gibbed.IO
Gibbed.ProjectData
Gibbed.Squish
NDesk.Options (Gibbed fork)

The trainer has been tested with Steam version 1.03 of Far Cry 2, and may not work with other versions.

By default the trainer uses "~/FarCry2_trainer" as a working directory to store game files, mod files, and Gibbed binaries.

Please make sure to begin with unmodded patch.dat and patch.fat files so that the trainer will be able to revert to an unmodded game if desired.

Changelog:

2018-04-28: v1.0.0 released with initial mod set


# Features

gamemodesconfig:
* unlock all weapons and equipment
* set cost of manuals to 1
* remove malaria
* unlimited sprint

28_pickups:
* make golden AK-47's respawn

30_player:
* change max slope climb cap

41_WeaponProperties:
* set weapon slot
* make silent
* remove accuracy spread
