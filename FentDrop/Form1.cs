using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;
using System.IO;

namespace FentDrop
{
    public partial class Form1 : Form
    {
        //personaje y listas de comida 
        FentBot personaje;
        List<List<Comida>> comida = new List<List<Comida>>();

        //reproductores de audio
        WMPLib.WindowsMediaPlayer player;
        string musicTmp;
        SoundPlayer efectoBueno;
        SoundPlayer efectoMalo;
        SoundPlayer efectoMorir;
        SoundPlayer efectoFrenesi;

        //imagenes a usar
        Image imgPollo;
        Image imgSandia;
        Image imgBanana;
        Image imgFenta;
        Image imgHitler;
        Image imgDerek;
        Image imgAlgodon;
        Image imgFloyd;

        Random rn;
        Keys direccion; //izq o der del personaje 

        Font fuente2 = new Font("Arial", 45, FontStyle.Bold);
        Font fuente3 = new Font("Arial", 55, FontStyle.Bold);
        Font fuente4 = new Font("Arial", 30, FontStyle.Bold);

        //contadores
        int puntos = 0;
        int contadorTmp = 0;
        int contadorFrenesi = 0;
        int niveles = 1; //iniciamos en el nivel 1
        int velocidad = 6; //velocidad inicial de la comida 
        int vidas = 5; //5 vidas 
        int numeroFilas = 7; //iniciales 
        int nivelesTmp = 1; 

        //booleanos
        bool comenzar = false;
        bool ayudaDiddy = false;
        bool pausa = false;
        bool mientrasFrenesi = false;
        bool acabasDeMorir = false;
        bool pasasNivel = false;

        //muros 
        Rectangle muro1;
        Rectangle muro2;
        Rectangle muro3;
        
        public Form1()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            UpdateStyles();

            InitializeComponent();
            this.KeyPreview = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //List<List<Comida>> copiaReferencias = comida.ToList();
            foreach(var filas in comida)
            {
                foreach(var comida in filas)
                {
                    if(!pausa)
                    {
                        comida.Dibujar(g);
                    }
                }
            }

            if(!comenzar) //solo si no ha comenzado el juego
            {
                string texto = "Da click para iniciar!!";
                SizeF size = g.MeasureString(texto, fuente3);
                int centroX = this.ClientSize.Width / 2;
                int centroY = this.ClientSize.Height / 2;
                Point p = new Point(
                    centroX - (int)(size.Width / 2),
                    centroY - (int)(size.Height / 2)
                );

                g.DrawString(texto, fuente3, Brushes.Yellow, p);
            }

            if(!acabasDeMorir)
            {
                personaje?.Dibujar(g); //si es nulo no dibuja
            }

            //dibujamos los muros
            g.FillRectangle(Brushes.DarkGreen, muro1);
            g.FillRectangle(Brushes.DarkGreen, muro2);
            g.FillRectangle(Brushes.DarkGreen, muro3);

            g.DrawString($"Nivel: {niveles} / Puntaje: {puntos} " +
                $"/ Vidas: {vidas}", fuente4, Brushes.White, 5, 15);

            if(acabasDeMorir)
            {
                string texto = "George Floyd murió...";
                SizeF size = g.MeasureString(texto, fuente3);
                int centroX = this.ClientSize.Width / 2;
                int centroY = this.ClientSize.Height / 2;
                Point p = new Point(
                    centroX - (int)(size.Width / 2),
                    centroY - (int)(size.Height / 2)
                );
                g.DrawString(texto, fuente3, Brushes.Red, p);
            }

            if(pasasNivel)
            {
                string texto = $"Nivel {niveles}...";
                SizeF size = g.MeasureString(texto, fuente3);
                int centroX = this.ClientSize.Width / 2;
                int centroY = this.ClientSize.Height / 2;
                Point p = new Point(
                    centroX - (int)(size.Width / 2),
                    centroY - (int)(size.Height / 2)
                );
                g.DrawString(texto, fuente3, Brushes.DarkBlue, p);
            }

            if(ayudaDiddy)
            {
                string texto = "Come lo que sea, eres INMUNE!! :)";
                SizeF size = g.MeasureString(texto, fuente2);
                int centroX = this.ClientSize.Width / 2;
                int centroY = this.ClientSize.Height / 2;
                Point p = new Point(
                    centroX - (int)(size.Width / 2),
                    centroY - (int)(size.Height / 2)
                );
                g.DrawString(texto, fuente2, Brushes.Yellow, p);
            }

            if(mientrasFrenesi)
            {
                string texto = $"El frenesí acaba en: {contadorFrenesi} s";
                SizeF size = g.MeasureString(texto, fuente4);

                g.DrawString(texto, fuente4, 
                    Brushes.Red, this.ClientSize.Width - size.Width, 15);
            }
        }

        //se inicia el juego: 
        private async void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            this.MouseClick -= Form1_MouseClick;

            efectoBueno.Stop();
            puntos = 0;
            contadorTmp = 0;

            //reproducimos la cancion de fondo
            musicTmp = Path.GetTempFileName() + ".mp3";
            File.WriteAllBytes(musicTmp, Properties.Resources.music);
            player = new WindowsMediaPlayer();
            player.URL = musicTmp;
            player.settings.setMode("loop", true);
            player.controls.play();

            //pausamos el timer de juego que fungia como pantalla
            //de carga 
            timerjuego.Stop();
            comida.Clear(); //limpiamos todas las filas de comida 
            comenzar = true;
            this.Invalidate();

            await Task.Delay(16);

            cargarComidaInicio(numeroFilas);
            personaje.esInmune = false;
            timerjuego.Start();

            this.Invalidate();
        }
        private void cargarComidaInicio(int numeroFilas)
        { 
            //creamos la primera fila
            int division = this.ClientSize.Width / 8;
            int cont = 0 + division;
            var comidaTmp = new List<Comida>();
            var ultimaFila = new List<Comida>();
            for(int i = 1; i <= 7; i++)
            {
                if(rn.Next() % 2 == 0) //si es par agregamos comida buena 
                {
                    switch(rn.Next(1, 4))
                    {
                        case 1:
                            comidaTmp.Add(new Comida(
                                cont - 55,
                                0 - rn.Next(100, 250),
                                imgBanana
                            ));
                            break;

                        case 2:
                            comidaTmp.Add(new Comida(
                                cont - 55,
                                0 - rn.Next(100, 250),
                                imgSandia
                            ));
                            break;

                        case 3:
                            comidaTmp.Add(new Comida(
                                cont - 55,
                                0 - rn.Next(100, 250),
                                imgPollo
                            ));
                            break;
                    }
                    comidaTmp[comidaTmp.Count - 1].esBuena = true;
                }
                else //sino agregamos comida mala 
                {
                    switch(rn.Next(1, 4))
                    {
                        case 1:
                            comidaTmp.Add(new Comida(
                                cont - 55,
                                0 - rn.Next(100, 250),
                                imgHitler
                            ));
                            break;

                        case 2:
                            comidaTmp.Add(new Comida(
                                cont - 55,
                                0 - rn.Next(100, 250),
                                imgDerek
                            ));
                            break;

                        case 3:
                            comidaTmp.Add(new Comida(
                                cont - 55,
                                0 - rn.Next(100, 250),
                                imgAlgodon
                            ));
                            break;
                    }
                    comidaTmp[comidaTmp.Count - 1].esBuena = false;
                }
                cont += division;
            }
            comida.Add(comidaTmp);

            //despues de agregar la primera fila ya podemos meter las 
            //otras 
            for(int i = 1; i < numeroFilas; i++)
            {
                ultimaFila = comida[comida.Count - 1];
                comidaTmp = new List<Comida>();

                for(int j = 0; j < 7; j++)
                {
                    if(rn.Next() % 2 == 0) //si es par agregamos comida buena 
                    {
                        switch(rn.Next(1, 4))
                        {
                            case 1:
                                comidaTmp.Add(new Comida(
                                    ultimaFila[j].rec.X,
                                    ultimaFila[j].rec.Y - rn.Next(100, 500),
                                    imgBanana
                                ));
                                break;

                            case 2:
                                comidaTmp.Add(new Comida(
                                    ultimaFila[j].rec.X,
                                    ultimaFila[j].rec.Y - rn.Next(100, 500),
                                    imgSandia
                                ));
                                break;

                            case 3:
                                comidaTmp.Add(new Comida(
                                    ultimaFila[j].rec.X,
                                    ultimaFila[j].rec.Y - rn.Next(100, 500),
                                    imgPollo
                                ));
                                break;
                        }
                        comidaTmp[comidaTmp.Count - 1].esBuena = true;
                    }
                    else //sino agregamos comida mala 
                    {
                        switch(rn.Next(1, 4))
                        {
                            case 1:
                                comidaTmp.Add(new Comida(
                                    ultimaFila[j].rec.X,
                                    ultimaFila[j].rec.Y - rn.Next(100, 500),
                                    imgHitler
                                ));
                                break;

                            case 2:
                                comidaTmp.Add(new Comida(
                                    ultimaFila[j].rec.X,
                                    ultimaFila[j].rec.Y - rn.Next(100, 500),
                                    imgDerek
                                ));
                                break;

                            case 3:
                                comidaTmp.Add(new Comida(
                                    ultimaFila[j].rec.X,
                                    ultimaFila[j].rec.Y - rn.Next(100, 500),
                                    imgAlgodon
                                ));
                                break;
                        }
                        comidaTmp[comidaTmp.Count - 1].esBuena = false;
                    }
                }
                comida.Add(comidaTmp);
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Right)
            {
                direccion = Keys.Right;
            }
            if(e.KeyCode == Keys.Left)
            {
                direccion = Keys.Left;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Right)
            {
                direccion = new Keys();
            }
            if(e.KeyCode == Keys.Left)
            {
                direccion = new Keys();
            }
        }

        //al cargar el formulario:
        private void Form1_Load(object sender, EventArgs e)
        {
            rn = new Random();

            this.Icon = Properties.Resources.icon;

            //creamos los muros
            muro1 = new Rectangle(10, 91, 20, this.ClientSize.Height - 91);
            muro2 = new Rectangle(this.ClientSize.Width - 30, 91, 
                20, this.ClientSize.Height - 91);
            muro3 = new Rectangle(10, this.ClientSize.Height - 20, this.ClientSize.Width - 30, 20);

            //cargamos las imagenes al inicar el juego             
            imgPollo = Properties.Resources.pollo;
            imgSandia = Properties.Resources.basquet; //cambia a basquet
            imgBanana = Properties.Resources.banana;
            imgFenta = Properties.Resources.fenta;
            imgAlgodon = Properties.Resources.ku; //cambia a ku kux klan
            imgDerek = Properties.Resources.derek;
            imgHitler = Properties.Resources.hitler;
            imgFloyd = Properties.Resources.g;

            //creamos al personaje 
            personaje = new FentBot((this.ClientSize.Width / 2) - 60, muro3.Y - 120, imgFloyd);
            personaje.esInmune = true;

            //cargamos los sonidos igual
            efectoBueno = new SoundPlayer(Properties.Resources.bueno);
            efectoBueno.Load();

            efectoMalo = new SoundPlayer(Properties.Resources.malo);
            efectoMalo.Load();

            efectoMorir = new SoundPlayer(Properties.Resources.morir);
            efectoMorir.Load();

            efectoFrenesi = new SoundPlayer(Properties.Resources.frenesi);
            efectoFrenesi.Load();

            cargarComidaInicio(50); //se crean las primeras comidas en esta funcion
            timerjuego.Start(); //inicia la pantalla de carga
            timerPersonaje.Start();
        }

        //timer para la animacion del personsaje
        private void timerPersonaje_Tick(object sender, EventArgs e)
        {
            //si va a la izquierda
            if(direccion == Keys.Left)
            {
                //si el personaje choca con el muro1 entonces
                if(personaje.rec.IntersectsWith(muro1))
                {
                    personaje.MoverDer(200);
                }

                if(mientrasFrenesi)
                {
                    personaje.MoverIzq(30);
                }
                else
                {
                    personaje.MoverIzq(20);
                }
            }
            //si va a la derecha
            else if(direccion == Keys.Right)
            {
                //si el personaje choca con el muro1 o muro2 entonces
                if(personaje.rec.IntersectsWith(muro2))
                {
                    personaje.MoverIzq(200);
                }

                if(mientrasFrenesi)
                {
                    personaje.MoverDer(30);
                }
                else
                {
                    personaje.MoverDer(20);
                }
            }

            this.Invalidate();
        }

        //timer del juego
        private async void timerjuego_Tick(object sender, EventArgs e)
        {            
            //removemos todas las filas en donde su comida sea >
            comida.RemoveAll(c => c.All(item => item.rec.Y > this.ClientSize.Height));

            if(comida.Count == 0 && comenzar) //si ya no hay elementos y ya
                                              //inició el juego pasamos de nivel
            {                
                if(!mientrasFrenesi) //si no es frenesi
                {
                    timerjuego.Stop();
                    timerPersonaje.Stop();
                    niveles++; //incrementamos
                    nivelesTmp++;

                    if(nivelesTmp == 9) //cada 10 niveles reseteamos las filas 
                    {
                        nivelesTmp = 1;
                        numeroFilas = 10;
                    }
                    else
                    {
                        numeroFilas += 6;
                    }

                    velocidad++;
                    pasasNivel = true;
                    this.Invalidate();

                    await Task.Run(() => cargarComidaInicio(numeroFilas));

                    timerjuego.Start();
                    timerPersonaje.Start();
                    pasasNivel = false;
                    return;
                }
                else //en caso de que si 
                {
                    timerjuego.Stop();
                    timerPersonaje.Stop();
                    timerFrenesi.Stop();
                    niveles++; //incrementamos
                    nivelesTmp++;

                    if(nivelesTmp == 9) //cada 10 niveles reseteamos las filas 
                    {
                        nivelesTmp = 1;
                        numeroFilas = 10;
                    }
                    else
                    {
                        numeroFilas += 6;
                    }

                    velocidad++;
                    pasasNivel = true;
                    this.Invalidate();

                    await Task.Run(() => cargarComidaInicio(numeroFilas));

                    timerjuego.Start();
                    timerPersonaje.Start();
                    timerFrenesi.Start();
                    pasasNivel = false;
                    return;
                }
            }
            else if(comida.Count == 0 && !comenzar) //en caso contrario seguimos en
                                                    //pantalla de carga 
            {

                this.MouseClick -= Form1_MouseClick;
                timerjuego.Stop();
                timerPersonaje.Stop();
                await Task.Run(() => cargarComidaInicio(50));
                timerjuego.Start();
                timerPersonaje.Start();
                this.MouseClick += Form1_MouseClick;
                return;
            }

            //vamos fila por fila buscando fenta      
            foreach(var filas in comida)
            {
                int n2 = filas.RemoveAll(comida =>
                    comida.hitBox.IntersectsWith(personaje.hitBox) && comida.esFenta);

                if(n2 > 0) //si al menos se elimino un elemento 
                            //entramos al frenesi
                {
                    timerjuego.Stop();
                    timerPersonaje.Stop();
                    player.controls.stop();
                    efectoBueno.Stop();
                    personaje.esInmune = true;
                    ayudaDiddy = true;
                    mientrasFrenesi = true;
                    pausa = true;
                    contadorFrenesi = rn.Next(10, 16);
                    this.Invalidate();

                    await Task.Delay(2000);

                    ayudaDiddy = false;
                    pausa = false;
                    timerjuego.Start();
                    timerPersonaje.Start();
                    timerFrenesi.Start();
                    efectoFrenesi.Play();
                    return;
                }
            }

            //eliminamos toda comida buena que choque con el personaje y le sumamos
            //los puntos
            int n = 0;
            foreach(var c in comida)
            {
                n = c.RemoveAll(comida => comida.hitBox.IntersectsWith(personaje.hitBox)
                    && comida.esBuena);

                if(n > 0 && comenzar) //si n > 0 minimio se elimino 1
                {
                    puntos += n;

                    if(!mientrasFrenesi) //solo si no es frenesi 
                    {
                        efectoBueno.Play(); 
                        contadorTmp += n;
                    }

                    //si llegamos a >= 25 puntos 
                    if(contadorTmp >= 30)
                    {                        
                        contadorTmp = 0;

                        //procedemos a posicionar el fenta en la matriz 
                        foreach(var fila in comida)
                        {
                            //buscamos el primer elemento dentro de la fila que
                            //su pos. en Y este entre -200 y -80
                            int j = fila.FindIndex(comida => comida.rec.Y >= -500 &&
                                comida.rec.Y <= -100);

                            if(j != -1) //en caso de encontrarlo
                            {   
                                fila[j] = new Comida(
                                    fila[j].rec.X,
                                    fila[j].rec.Y,
                                    imgFenta
                                );
                                fila[j].esFenta = true;

                                filaTmp = fila;
                                comidaTmp = fila[j];
                                timerParpadeo.Start();
                                break;
                            }
                        }
                    }
                }
            }

            //eliminamos comida mala dependiendo si es frenesi
            foreach(var filas in comida)
            {
                //en caso de que si sea sumamos puntos
                int n3 = filas.RemoveAll(c => c.hitBox.IntersectsWith(personaje.hitBox) &&
                    !c.esBuena && personaje.esInmune);
                if(n3 > 0 && comenzar) 
                {
                    puntos += n3;
                }

                //en caso de que no quitamos vidas 
                int n4 = filas.RemoveAll(c => c.hitBox.IntersectsWith(personaje.hitBox)
                    && !c.esBuena && !personaje.esInmune);
                if(n4 > 0)
                {
                    efectoMalo.Play();
                    
                    vidas--;

                    if(vidas == 0)
                    {
                        timerjuego.Stop();
                        timerPersonaje.Stop();
                        player.controls.stop();
                        efectoBueno.Stop();
                        timerParpadeo.Stop();
                        comida.Clear();
                        acabasDeMorir = true;
                        this.Invalidate();

                        //cuando el sonido y la funcion terminan
                        await Task.Run(() => efectoMorir.PlaySync());
                        await Task.Run(() => cargarComidaInicio(50));

                        //reseteamos el juego 
                        personaje.esInmune = true;
                        acabasDeMorir = false;
                        comenzar = false;
                        mientrasFrenesi = false;
                        pausa = false;
                        ayudaDiddy = false;
                        pasasNivel = false;
                        numeroFilas = 7;
                        niveles = 1;
                        puntos = 0;
                        contadorTmp = 0;
                        velocidad = 6;
                        vidas = 5;
                        nivelesTmp = 1;
                        personaje.CambiarPos((this.ClientSize.Width / 2) - 60,
                            this.ClientSize.Height - 130);
                        timerjuego.Start();
                        timerPersonaje.Start();
                        this.MouseClick += Form1_MouseClick;
                        this.Invalidate();
                        return;
                    }
                }

                foreach(var item in filas)
                {
                    item.MoverAbajo(velocidad);
                }
            }

            this.Invalidate();
        }
        private void timerFrenesi_Tick(object sender, EventArgs e)
        {   
            contadorFrenesi--;

            if(contadorFrenesi == 0)
            {
                timerFrenesi.Stop();
                personaje.esInmune = false;
                mientrasFrenesi = false;
                efectoFrenesi.Stop();
                player.controls.play();
            }
        }

        List<Comida> filaTmp = new List<Comida>();
        Comida comidaTmp = new Comida();
        private void timerParpadeo_Tick(object sender, EventArgs e)
        {
            if(!comida.Contains(filaTmp)) //en caso de que ya no esté
                                          //en la lista
            {
                timerParpadeo.Stop();
                filaTmp = null;
                comidaTmp = null;
                return;
            }

            if(comidaTmp.UnoSi) //si está prendido 
            {
                comidaTmp.UnoSi = false;
            }
            else //en caso de estar apagado 
            {
                comidaTmp.UnoSi = true;
            }

            this.Invalidate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if(File.Exists(musicTmp))
            {
                File.Delete(musicTmp);  
            }
        }
    }
}
