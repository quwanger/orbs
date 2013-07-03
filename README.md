Project Rollout
================

IMD Senior Project.

Notes:
Please check back for PR's and Issues regularily, (once every two days). Check out and import the project with Unity.


Decal System Tutorials
-------

We are using a free tool from that asset store named Decal System. Please watch www.youtube.com/watch?v=C87xkHj8jHM‎to get a hang of it.
Also, we should credit Eidelweiss Interactive in our credits. They released a super powerful tool for free. 


Testing Scene Transitions and game flow
-------

We use a persistence manager that is created in the init scene. If you want to debug transitions, or play the game, open the init scene and play from there or build and run.

Testers Union and Splunk Storm
-------

To Log To testers Union, use 
`StartCoroutine(GameObject.Find("Logger").GetComponent<TTSTestersUnion>().logEvent(title, params));`

The title of your event should be unique and searchable, and in all caps. for example:
`"SESSION_START"` or `"SESSION_END"`

The params parameter lets you specify extra data with your log, for example if I wanted to log Invalid Keypresses, I could Log the key they hit by accident:
`logEvent("INVALID_KEYPRESS", "key=" + Input.GetKeyDown().toString()"`

You can specify multiple params with a whitespace delimiter:
`logEvent("INVALID_KEYPRESS", "key=" + Input.GetKeyDown() + " tag=ingame");`

Here's some stuff that's logged by default in evey message:

- UTC Timestamp Of Event
- Name from database
- Gender
- Age
- If you are in edit mode (debug mode)
- Token


Here's some things that would be cool to do with it:

- return a session id at the start, and tag all events with it.
- send events on exceptions or error logs.
- send events on crashes.


Pivotal Integreation
-------

We now integrate with pivotal. If you include a pivotal story id in a branch name if will be added to the story as a comment:

`Branch Name: 1923049-new-feature`

You can also tag stories in commits with the number in the commit message, formatted like this:

`[#1238492] Fixes some problem`

We should have a pivotal story for every branch. If you are doing something that does not have a pivotal story associated with it, create one! 

