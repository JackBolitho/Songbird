# Songbird
This is my DALI lab developer challenge. It is an arcade game called "Songbird" created in Unity. 
Playing the game requires a microphone, since it takes sound input to control the character.
I recommend playing in a quiet room for less sonic interference!

A constant barage of obstacles come at the player, who is represented as a little bird. Players must sing into the microphone in order to avoid obstacles.
The higher a player sings, the higher they move. The lower a player sings, the lower they move. 

"Songbird" takes audio-spectral data from microphone input, calculates the fundamental frequency of said data, and moves the player accordingly. 
When the player first sings, the frequency of their first note is recorded as the movement baseline. All subsequent movement is dependent on that initial note;
the character will move depending on the relationship in cents - a logarthimic unit of measurement used for musical intervals
(an octave has 1200 equally divided cents) - between the starting note and the current note. 
