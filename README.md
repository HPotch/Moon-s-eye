This is my project for The Outer Wilds assignment at HKU Games year 1.

The game features dialogue, which is automatically decoded in-game for easy editing. In Unity, you can assign any text file to an NPC to make it talk. These are the features of the dialogue:
1) Talks - these are texts that the NPC says
2) Questions - these are answers that the player can choose
3) References - You can pass any Talk or Question after any of those. This will be the follow-up for the dialogue.
4) Start Talks - These are the talks that the conversation starts on
5) End Talks / Questions - When the player presses confirm after these Talks or Questions, the dialogue is over.

This is how you should assign them in such a text file (I recommend you look at the example below):
1) Start every line with either a '/' for a Talk, or a '?' for a question.
2) Put a '>' after that to mark the beginning. If you enter multiple beginnings, only the first one is made the beginning and the following >s are included in the keys.
3) Then, enter the key this can contain every character but ':' and ';'. This key is not visable to the player but is used for referencing this line. Duplicate keys aren't allowed!
4) Enter a ':' to mark the end of a key.
5) Enter the text you want the Question or Talk to be, this is visible to the player. This can contain any character but ';'.
6) Enter a ';' to mark the end of the text.
7) Enter a '<' to mark the end of the dialogue. Otherwise, continue to step 8.
8) If the line is a Talk you should define whether the next text is a Talk or a Question, use the characters '/' and '?' for a Talk and Question respectively. If this line is a Question, continue to step 9 (Questions can only be followed by talks).
9) Enter the reference key for the next Talk or Question. If you want to add multiple Question choices, space them apart with a ';'.

I understand that that is a bit much, so here is an example:

```
/>A:Hi, I'm max!;?1;2;3;
/B:I'm one of the moonfolk;?4;5;6;7
/C:That's not very nice;/D
/D:I don't want to talk to you anymore;<
/E:Thanks, that's nice! I gotta go though, see you later.;<
/F:I try to be nice but with that attitude I don't like to see you anymore.;<
/G:Well, were did invent boats to get here, so I guess we're better after all.;/H
/H:If you don't mind, I gotta go.;?8;9
/I:It wasn't a question...;<
/J:Ermm.. My parents named me that way... What's your name?;?10;11
/K:That's too bad;<

?1:Hi Max!;B
?2:What a weird name;C
?3:Could you repeat that;A
?4:I've heard moonfolk are pretty cool;E
?5:Moonfolk scare me;F
?6:I'm from Eye, I think it's the better folk;G
?7:But why?;J
?8:Sure;<
?9:No;I
?10:I don't have a name;K
?11:Great question;K
```

Commented version:

```
/>A:Hi, I'm max!;?1;2;3; <-- The ´>' marks the beginning of the conversation
/B:I'm one of the moonfolk;?4;5;6;7 <-- '?4;5;6;7' tells the code to reference the Questions 4, 5, 6 and 7 as next options
/C:That's not very nice;/D <-- '/D' tells the code to run the Talk with key 'D' next. Notice how this is only one Talk, multiple talks cannot be given.
/D:I don't want to talk to you anymore;< <- '<' marks the end of the conversation
/E:Thanks, that's nice! I gotta go though, see you later.;<
/F:I try to be nice but with that attitude I don't like to see you anymore.;<
/G:Well, were did invent boats to get here, so I guess we're better after all.;/H
/H:If you don't mind, I gotta go.;?8;9
/I:It wasn't a question...;<
/J:Ermm.. My parents named me that way... What's your name?;?10;11
/K:That's too bad;<

?1:Hi Max!;B <- This is a Question - it starts with a '?'
?2:What a weird name;C <- Notice how these Questions do not mark a Question or Talks as a next step - it is always one Talk.
?3:Could you repeat that;A
?4:I've heard moonfolk are pretty cool;E
?5:Moonfolk scare me;F
?6:I'm from Eye, I think it's the better folk;G
?7:But why?;J
?8:Sure;< <- Questions can also mark the end of the conversation
?9:No;I
?10:I don't have a name;K
?11:Great question;K
```

Tips:
1) As you can see, I like to seperate my Talks and Questions from eachother. However, you can also declare them in any other order.
2) I named the references keys for Talks alphabetically, Questions are numerically. You can call them anything you like but I like to do it this way to avoid having duplicates. (after Z I'd name it 'AA')
3) Don't worry to test out a bit, I've implemented error codes for the most common errors.
4) Look back at the example if you get stuck!

Good luck!
