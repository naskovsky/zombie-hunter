using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zombie_Hunter
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 2;
        int hpdropped = 0;
        Random randNum = new Random();
        int score;
        List<PictureBox> zombiesList = new List<PictureBox>();
        Timer timer;


        public Form1()
        {
            InitializeComponent();
            InitTimer();
            RestartGame();
        }

        // timer method for hp crates
        public void InitTimer()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer1_Tick);
            timer.Interval = 10000; // in miliseconds
            timer.Start();
        }

        // timer ticker for hp crates
        private void timer1_Tick(object sender, EventArgs e)
        {
            // if the players health is lower or equal to 80 hp, a crate is dropped every 10 seconds,
            // also if there are more than 3 hp crates on the field at the same time the condition will be false
            // and there wouldn't be another crate dropped, unless the player collects one, and the overall number of hp
            // crates is 2, then the timer starts again and a hp crate is dropped
            if (playerHealth <= 80 && hpdropped < 3)
            {
                DropHealthPoints();
                hpdropped += 1;
            }
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                GameTimer.Stop();
                MessageBox.Show("GAME OVER! \nPRESS ENTER TO START AGAIN!");
            }

            // if the score reaches 25, the speed of the zombies increase by 1
            if (score >= 25)
            {
                zombieSpeed = 3;
            }

            txtAmmo.Text = "Ammo: " + ammo;
            txtScore.Text = "Kills: " + score;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }
            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }
            if (goUp == true && player.Top > 45)
            {
                player.Top -= speed;
            }
            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                // checks if the player walks over an ammo box
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    // if the player has colided with an ammo box
                    // increase the overall ammo by 5.
                    // ammo only spawns when the player has 0 ammo left.
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                // checks if player walks over an hp crate
                if (x is PictureBox && (string)x.Tag == "hp")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();

                        // if the player has 100 and walks over an hp crate
                        // it shouldn't add to his health
                        if (playerHealth == 100)
                        {
                            playerHealth = playerHealth + 0;
                        }

                        // if the player has over 90 health and walks over an hp crate
                        // it should only give him the ammount left when subtracting the max hp allowed
                        // and his current hp, so 100 - playerHealth
                        else if (playerHealth > 90)
                        {
                            int remaining = 100 - playerHealth;
                            playerHealth += remaining;
                        }

                        // if the player has 90 or less health it should add 10 hp to his current hp
                        else
                        {
                            playerHealth += 10;
                        }

                        // every time the player walks over and collects the hp crate the
                        // counter for hp crates dropped, is decrementing by 1,
                        // this helps the conditional state that tracks if there are more than 3 crates on the field
                        hpdropped -= 1;
                    }
                }

                // checks if the player colides with a zombie
                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    // if the player has colided with a zombie
                    // decrease the health of the player, by 1.
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                    }

                    if (x.Left > player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }

                    if (x.Left < player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    if (x.Top > player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }

                    if (x.Top < player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }
                }

                // this checks if the bullet has colided with any of the zombies.
                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;

                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            zombiesList.Remove(((PictureBox)x));
                            MakeZombies();
                        }
                    }
                }

            }

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (gameOver == true)
            {
                return;
            }

            // player image is rotated to the left
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }

            // player image is rotated to the right
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            // player image is rotated to the top
            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            // player image is rotated to the bottom
            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }


            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                // calls a function that fires the bullet in which
                // direction the player is turned.
                ShootBullet(facing);

                // checks if the ammo is less than 1, or 0
                // and calls a function that randomly drops ammo on the field.
                if (ammo < 1)
                {
                    DropAmmo();
                }

            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }
        }

        // method for the trajectory of the bullet,
        // gets a direction as a parameter
        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        // method for spawning zombies
        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.Left = randNum.Next(0, 900);
            zombie.Top = randNum.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            player.BringToFront();
        }

        // method for spawning ammo crates
        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = randNum.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            this.Controls.Add(ammo);

            ammo.BringToFront();
            player.BringToFront();
        }


        // method for spawning hp crates  
        private void DropHealthPoints()
        {
            PictureBox hp = new PictureBox();
            hp.Image = Properties.Resources.hp;
            hp.SizeMode = PictureBoxSizeMode.AutoSize;
            hp.Left = randNum.Next(10, this.ClientSize.Width - hp.Width);
            hp.Top = randNum.Next(60, this.ClientSize.Height - hp.Height);
            hp.Tag = "hp";
            this.Controls.Add(hp);

            hp.BringToFront();
            player.BringToFront();
        }

        private void RestartGame()
        {
            player.Image = Properties.Resources.up;
            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();
            for (int i = 0; i < 3; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;

            playerHealth = 100;
            score = 0;
            ammo = 10;
            zombieSpeed = 2;

            GameTimer.Start();
        }
    }
}
