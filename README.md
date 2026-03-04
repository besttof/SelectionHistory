# Selection History

Ever spent time searching back to that one prefab you were editing a minute ago, but lost it because you had to look at
some texture import settings? Wouldn't it be nice to restore that selection?

That is exactly what this package offers: a way to keep track of your selections and navigate back and forth between
them:

| Default Shortcut       | Action                  |
|------------------------|-------------------------|
| `Ctrl/Cmd + Shift + -` | Previous Selection      |
| `Ctrl/Cmd + Shift + =` | Next Selection          |
| `Ctrl/Cmd + Shift + \` | Clear Selection History |

## Settings

Some settings are available in as User Preferences:

| Setting                         | Description                                                                                                                                                                                                                    | Default Value |
|---------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------|
| Capacity                        | The maximum number of selections to keep in history.                                                                                                                                                                           | 25            |
| Clear Selection On Scene Change | Clear the selection on destructive scene changes (i.e. when all loaded scenes are replaced)                                                                                                                                    | True          |
| Ignore Empty Selections | Only keep track of selections that have something selected                                                                                                                                                                     | False         |
| Selection Mode | Either `Fast And Naive` or `Slow And Correct`. Slow is considerably slower and uses a different system to reference objects which allows references to correcly restore after scene unload or other context switches. | `Fast And Naive` |