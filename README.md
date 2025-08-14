## Gameplay requirements

* Waves:
    * Spawn instantly 4 asteroids, and increases by 1 each wave
    * They always spawn as big asteroids
    * A new wave begins once the screen has been cleared of all asteroids and actively flying saucers
    * Each wave spawns increasingly faster asteroids, starting with 1 and increasing by 1 each wave
* Asteroids:
    * Big asteroids spawn 2 medium ones once destroyed
    * Medium asteroids spawn 2 small ones once destroyed
    * Asteroids don't collide with each other
    * Smaller asteroids always spawn on the same rough direction, within a cone of 120 degrees (?)
* The player can move in any direction
    * The player can move forward and backward
    * The player can turn left and right
    * The player can thrust forward
    * The player can turn left and right
* Bullets have a limited range so it technically can't hit its own shooter due to that
    * Saucer bullets can hit asteroids
* Player can shoot bursts of 4 bullets
    * There's a limited rate of fire
    * There's a small cooldown between bursts
* Two flying saucers appear periodically on the screen
    * The "big saucer" shoots randomly and poorly
    * The "small saucer" fires frequently at the ship
        * As the player's score increases, the angle range of the shots from the small saucer diminishes until the saucer fires extremely accurately.
    * After reaching a score of 40,000, only the small saucer appears
    * They always move from one side to the other, randomly moving up and down diagonally
    * Both can appear already in the first wave
    * They can get destroyed asteroids, breaking asteroids in the process
* The player starts with 3 lives
    * Every 10k points, the player gains 1 life
* The player can teleport the ship, causing it to disappear and reappear in a random location on the screen
    * It's possible to teleport into asteroids, killing the ship
* Points are only given by player bullets:
    * Big asteroid: 20 points
    * Medium asteroid: 50 points
    * Small asteroid: 100 points
    * Big saucer: 200 points
    * Small saucer: 1000 points


Lurking exploit