# AntiSurfaceCamping

Stops people from staying on the surface for too long by damagin them.

## Config

| Name                  | Description                                                              | Default                                                              | Type           |
|-----------------------|--------------------------------------------------------------------------|----------------------------------------------------------------------|----------------|
| IsEnabled             | If the Plugin should start or not                                        | true                                                                 | Bool           |
| Damage                | The damage camping players get.                                          | 0.5                                                                  | Float          |
| TimeUntilNotice       | The time until players get notified that they will soon get damage.      | 30                                                                   | Int            |
| NoticeMessage         | The message to display when players will soon start getting damage.      | Please do not camp on the surface Zone. You will soon get damage!    | String         |
| DamageMessage         | The message to display when players get damage because they are camping. | You are getting damage because you are camping on the surface.       | String         |
| FatalDamageMessage    | The message to display when players get fatal damage.                    | You are getting fatal damage because you are camping on the surface. | String         |
| TimeUntilDamage       | The time until players get damage.                                       | 60                                                                   | Int            |
| DisableIfWarhead      | If the warhead should disable the timer.                                 | true                                                                 | Bool           |
| DisableIfSCPOnSurface | If a SCP on the surface should disable the timer.                        | true                                                                 | Bool           |
| TimeUntilFatalDamage  | The time until fatal damage starts                                       | 100                                                                  | Int            |
| FatalDamage           | The amount of fatal damage people will recieve.                          | 5                                                                    | Float          |
| immuneRoles           | Roles which wont get camping checked.                                    | Tutorial                                                             | List<RoleType> |
