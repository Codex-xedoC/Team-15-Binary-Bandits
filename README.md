  # CSCE-4901-Sponsor-Project
The following game design plan has a much larger scope than the project that is expected. I will try to add as much as I think I can. 

Guides & Tools:
- Guide for generating a basic maze: https://medium.com/@Scriptie/3d-maze-generation-in-unity-ec36415b659a

Game Overview:
The VR Maze Quiz Game is a dynamic blend of quiz-solving, maze exploration, and survival. Players must navigate a maze filled with doors that hold quiz questions, while being pursued by a monster that serves as a time limit. Players must use strategy to choose the right algorithm to navigate the maze, answer questions correctly to gain a low score, and ultimately reach the end of the maze with the least score possible.

Core Mechanics:
1. Algorithm-Based Door Access
- At the start of each maze, players choose an algorithm that determines which doors they can access.
- Each door has a quiz question related to a specific topic. Players can only enter the doors permitted by their chosen algorithm.
- Once the player answers the question, they will see the score associated with that door.
- The objective is to reach the end of the maze with the lowest score possible. Incorrect answers will leave players without a score for that door, forcing them to guess for their next move.
2. Level Progression and Unlockable Items
- The game features a progression system with different levels and difficulty settings.
- Winning levels unlock new items, which can be used to assist in future runs. These items could include tools to slow down the monster, extra hints for questions, or shortcuts through the maze.
3. Endgame Word Puzzle
- At the end of the maze, players are presented with a word puzzle.
- Depending on how many correct doors the player chose, they receive words related to the answer for the final question.
- Incorrect doors will provide misleading words, making it more difficult to solve the final puzzle.
- The player must pick the correct word to complete the final challenge.
4. Monster Chase Mechanic
- The game has a monster that chases players through the maze, functioning as the time limit.
- The monster's speed and behavior change based on difficulty level—higher difficulties mean faster pursuit and a shorter time limit.
- If the monster catches the player, they will face a pop quiz as a last-chance opportunity to continue. Players must answer all questions correctly to proceed, but this can only happen once per game.
- The environment and music dynamically shift as the monster gets closer, creating an intense atmosphere. If the monster reaches the player's location, the entire area goes pitch black, and players must rely on sound cues to avoid the monster while trying to answer questions at doors.
5. Pre-Maze Information
- Before entering the maze, players receive limited information or a preview of the maze layout.
- This pre-maze information helps players decide which algorithm to choose and develop their strategy before starting the game.
  
Game Features:
6. Monster Variations
- Each level introduces a unique monster with different abilities, visuals, and strategies.
- The monsters have unique music and different quiz content for the pop quizzes when they catch the player.
- These monster variations provide fresh challenges as the player progresses through levels.
7. Random Events and Multiplayer Mode
- Random events can occur throughout the game, such as temporary maze alterations, power-ups, or sudden pop quizzes.
- These events add unpredictability while maintaining a balance to avoid feeling unfair or out of place.
- A versus mode allows players to compete against each other or CPU opponents in timed maze runs, adding a competitive layer to the gameplay.
8. Interactive Movement
- Players may encounter sections where they must swing on vines or crawl through tunnels to progress between doors.
- These interactive movement mechanics add variety and create a more immersive VR experience.
9. Multi-Question Doors
- Some doors require players to answer multiple questions to proceed.
- These doors are marked with a unique color to indicate a higher difficulty.
- Successfully answering all questions on these doors will reward players with extra points or special items.
  
Game Modes:
1. Single Player Mode
- Players progress through levels, each containing a unique maze layout and monster.
- The goal is to reach the end of the maze with the lowest possible score, avoiding the monster along the way.
2. Multiplayer Mode
- Players compete against each other or CPU opponents to complete the maze with the lowest score or the fastest time.
- Versus mode includes additional challenges, such as player sabotage or cooperative elements for teams.
  
Visual and Audio Design:
1. Maze Design
- Mazes are visually distinct, with doors clearly marked to indicate their difficulty and the type of question.
- The environments should shift dynamically based on the player's situation, especially when the monster is close.
2. Monster Design
- Monsters have unique designs, each with a specific movement style and visual aesthetic.
- The distinct soundtracks for each monster will provide audio cues, intensifying the player's sense of urgency.
3. Dynamic Audio
- Music and sound effects will change depending on the proximity of the monster to the player.
- Ambient sounds like footsteps, breathing, and distant growls will add to the suspenseful experience.
  
