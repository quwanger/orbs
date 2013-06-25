Project Rollout
================

IMD Senior Project.

Notes:
Please check back for PR's and Issues regularily, (once every two days). Check out and import the project with Unity.

Commit notes:
Try not to stage files that have a plib extension. or anything that definetly isnt related to what you were changing. WE WILL KNOW. see me @JakeCataford to resolve merge conflicts and stuff.


Testing Scene Transitions and game flow
-------

We use a persistence manager that is created in the init scene. If you want to debug transitions, or play the game, open the init scene and play from there or build and run.


Pivotal Integreation
-------

We now integrate with pivotal. If you include a pivotal story id in a branch name if will be added to the story as a comment:

`Branch Name: 1923049-new-feature`

You can also tag stories in commits with the number in the commit message, formatted like this:

`[#1238492] Fixes some problem`

Lastly you can tag them in PR's the same way as above by putting it in the title

*[#123456] Adds Every Feature*
