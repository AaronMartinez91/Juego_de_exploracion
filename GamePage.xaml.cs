namespace B2ACT2Juego_de_exploracion;

public partial class GamePage : ContentPage
{
    // El jugador empieza siempre en la habitación 1
    int habitacion = 1;

    // Clave: habitación, Valor: array [N, S, E, O] (-1 = no hay salida)
    Dictionary<int, int[]> mapa = new Dictionary<int, int[]>
    {
        { 1,  new int[] {2, -1, -1, -1}},  // Porche
        { 2,  new int[] {3, 1, -1, -1}},  // Vestíbulo
        { 3,  new int[] {-1, 2, 8, 4}},  // Salón
        { 4,  new int[] {-1, 5, 3, -1}},  // Pasillo librería
        { 5,  new int[] {4, 6, -1, -1}},  // Librería
        { 6,  new int[] {5, -1, -1, -1}},  // Despacho
        { 7,  new int[] {-1, -1, 5, -1}},  // Cámara secreta
        { 8,  new int[] {-1, 10, 9, 3}},  // Comedor
        { 9,  new int[] {-1, -1, -1, 8 }},  // Cocina
        { 10, new int[] {8, -1, 11, -1}},  // Pasillo dormitorio
        { 11, new int[] {-1, 13, 12, 10}},  // Dormitorio
        { 12, new int[] {-1, -1, -1, 11}},  // Balcón
        { 13, new int[] {11, -1, -1, -1}}   // Lavabo
    };

    // Lista de nombres de los archivos para las imágenes de las habitaciones
    string[] imagenesHabitacion = 
    {
        "",                          
        "h01_porche.png",
        "h02_vestibulo.png",
        "h03_salon.png",
        "h04_pasillo_libreria.png",
        "h05_libreria.png",
        "h06_despacho.png",
        "h07_camara_secreta.png",
        "h08_comedor.png",
        "h09_cocina.png",
        "h10_pasillo_dormitorio.png",
        "h11_dormitorio.png",
        "h12_balcon.png",
        "h13_lavabo.png"
    };

    // Lista de los nombres de las habitaciones que se muestran en el cuadro de texto
    string[] nombresHabitacion = 
    {
        "",
        "el porche",
        "el vestíbulo",
        "el salón",
        "el pasillo hacia la librería",
        "la librería",
        "el despacho",
        "la cámara secreta",
        "el comedor",
        "la cocina",
        "el pasillo hacia el dormitorio",
        "el dormitorio",
        "el balcón",
        "el lavabo"
    };

    // Objetivo (1/2/3), el juego empieza por el primero, evidentemente
    int objetivo = 1;

    // jugador 0=llave, 1=libro, 2=gato, 3=pluma
    int itemJugador = -1;

    // personajes (el nº coincide con el de la lista de habitaciones, representa dónde están)
    int habTio = 6;
    int habTia = 11;
    int habCocinero = 9;

    // ítems (habitación en la que están)
    int habLlave = -1;
    int habLibro = -1;
    int habGato = -1;
    int habPluma = -1;

    // condiciones
    bool libroVisto = false;
    bool llaveVista = false;
    bool habladoCocinero = false;
    bool comidaHecha = false;
    bool tioEscribiendo = false;
    bool pajaroVisto = false;
    bool puertaCamaraAbierta = false;
    bool puertaCamaraVista = false;

    public GamePage()
	{
		InitializeComponent();        
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ActualizarVista();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    void ActualizarVista()
    {
        // Cambiar imagen de la habitación
        imgHabitacion.Source = imagenesHabitacion[habitacion];

        // Mostrar botones de dirección
        btnNorte.IsVisible = mapa[habitacion][0] != -1;
        btnSur.IsVisible = mapa[habitacion][1] != -1;
        btnEste.IsVisible = mapa[habitacion][2] != -1;
        btnOeste.IsVisible = mapa[habitacion][3] != -1;

        // Casos especiales
        if (habitacion == 5 && puertaCamaraVista)
        {
            btnOeste.IsVisible = true;
        }

        // Resetear visibilidad de todo lo opcional antes de comprobar
        imgPersonaje.IsVisible = false;
        imgItemHabitacion.IsVisible = false;
        imgItemJugador.IsVisible = false;

        // Mostrar texto de dónde se encuentra el jugador.
        infoText.Text = "Entras en " + nombresHabitacion[habitacion] + ".";

        // mostrar personaje en las habitaciones
        if(habTio == habitacion)
        {
            imgPersonaje.Source = "p02_senyor.png";
            imgPersonaje.IsVisible = true;
        }
        if(habTia == habitacion)
        {
            imgPersonaje.Source = "p01_senyora.png";
            imgPersonaje.IsVisible = true;
        }
        if(habCocinero == habitacion)
        {
            imgPersonaje.Source = "p03_cocinero.png";
            imgPersonaje.IsVisible = true;
        }

        // mostrar items en las habitaciones
        if(habLlave == habitacion)
        {
            imgItemHabitacion.Source = "i01_llave.png";
            imgItemHabitacion.IsVisible = true;
        }
        if(habLibro == habitacion)
        {
            imgItemHabitacion.Source = "i04_libro.png";
            imgItemHabitacion.IsVisible = true;
        }
        if(habGato == habitacion)
        {
            imgItemHabitacion.Source = "i02_gato.png";
            imgItemHabitacion.IsVisible = true;
        }
        if(habPluma == habitacion)
        {
            imgItemHabitacion.Source = "i03_pluma.png";
            imgItemHabitacion.IsVisible = true;
        }

        // mostrar item que lleva el jugador
        if(itemJugador == 0)
        {
            imgItemJugador.Source = "i01_llave.png";
            imgItemJugador.IsVisible = true;
        }
        if(itemJugador == 1)
        {
            imgItemJugador.Source = "i04_libro.png";
            imgItemJugador.IsVisible = true;
        }
        if(itemJugador == 2)
        {
            imgItemJugador.Source = "i02_gato.png";
            imgItemJugador.IsVisible = true;
        }
        if(itemJugador == 3)
        {
            imgItemJugador.Source = "i03_pluma.png";
            imgItemJugador.IsVisible = true;
        }

    }

    // Botones de direcciones

    void btnNorte_Clicked(object sender, EventArgs e)
    {
        habitacion = mapa[habitacion][0];
        ActualizarVista();
    }

    void btnSur_Clicked(object sender, EventArgs e)
    {
        habitacion = mapa[habitacion][1];
        ActualizarVista();
    }

    void btnEste_Clicked(object sender, EventArgs e)
    {
        // El objetivo 1 termina cuando se cumpla la condición de mover a la tía, por lo que no necesito booleano aquí
        if(habitacion == 10 && objetivo == 1)
        {
            infoText.Text = "Tu tía sigue durmiendo al otro lado. Un sueño profundo, casi antinatural. En tu familia siempre dijeron que solo había dos cosas capaces de arrancarla de la cama: un incendio y la hora de comer. Rezas para que sea la segunda.";
        }
        else
        {
            habitacion = mapa[habitacion][2];
            ActualizarVista();
        }
    }

    void btnOeste_Clicked(object sender, EventArgs e)
    {
        if(habitacion == 5 && puertaCamaraVista && !puertaCamaraAbierta)
        {
            infoText.Text = "La puerta está cerrada con llave.";
        }
        else if (habitacion == 5 && puertaCamaraAbierta)
        {
            habitacion = 7;
            ActualizarVista();
        }
        else
        {
            habitacion = mapa[habitacion][3];
            ActualizarVista();
        }
            
    }

    // Botones de acciones
    void btnObservar_Clicked(object sender, EventArgs e)
    {
        if(habitacion == 6)
        {
            infoText.Text = "Observas una llave cerca de tu tío";
            llaveVista = true;
            habLlave = 6;

            // la muestro al instante de observar, no espero a actualizar página
            imgItemHabitacion.Source = "i01_llave.png";
            imgItemHabitacion.IsVisible = true;
        }
        else if(habitacion == 5 && !puertaCamaraVista)
        {
            infoText.Text = "Observas una puerta oculta... ¿Podría ser una cámara secreta?";
            puertaCamaraVista = true;
            btnOeste.IsVisible = true;
        }
        else if (habitacion == 10 && !comidaHecha && objetivo == 1)
        {
            infoText.Text = "Acercas el ojo a la cerradura. Un error que lamentarás el resto de tu vida. Tu tía se remueve entre las sábanas y el pijama no deja demasiado a la imaginación. Cierras los ojos, pero ya es demasiado tarde. Algunas imágenes no se pueden desver.";
        }
        else if (habitacion == 10 && comidaHecha && objetivo == 1)
        {
            infoText.Text = "La puerta del dormitorio se abre de golpe. Tu tía sale disparada por el pasillo en dirección al comedor, con el pijama ondeando al viento de forma que nunca podrás olvidar. Grita algo sobre el cocido. No levantas la vista del suelo.";
            habTia = 8;
            objetivo = 2;
        }
        else if (habitacion == 2)
        {
            infoText.Text = "Sobre la chimenea cuelga un enorme retrato al óleo de tu tío. Está pintado con una sonrisa demasiado amplia, los ojos demasiado abiertos. El pintor debió de sentirse muy incómodo durante las sesiones.";
        }
        else if (habitacion == 4)
        {
            infoText.Text = "El pasillo huele a algo que no consigues identificar. Dulzón, pero con un fondo que te revuelve el estómago. Bajas la vista y ves que la moqueta tiene manchas oscuras en varios puntos. Alguien intentó limpiarlas. No del todo bien.";
        }
        else if (habitacion == 3 && !libroVisto)
        {
            infoText.Text = "Ves un libro de recetas sobre la mesa. Qué cosa tan mundana en una mansión tan... peculiar.";
            libroVisto = true;
            habLibro = 3;
            imgItemHabitacion.Source = "i04_libro.png";
            imgItemHabitacion.IsVisible = true;
        }
        else
        {
            infoText.Text = "No observas nada relevante";
        }
    }    
    void btnHablar_Clicked(object sender, EventArgs e)
    {
        if (habitacion == 6)
        {
            if(!habladoCocinero)
            {
                infoText.Text = "Tu tío te saluda efusivamente, su contacto excesivo te pone un poco nervioso, pero no le das importancia.";
            }
            else
            {
                infoText.Text = "Tío: ¿Buscas el libro de recetas? Diría que la última vez que lo vi estaba en el salón.";
            }
        }
        else if (habitacion == 9)
        {
            infoText.Text = "Cocinero: Llevo horas intentando recordar el ingrediente especial de esta receta... Sin él no puedo terminar el plato. Y sin el plato... mejor ni pensarlo.";
            habladoCocinero = true;
        }
        else
        {
            infoText.Text = "No hay nadie en la habitación";
        }
    }    
    void btnRecoger_Clicked(object sender, EventArgs e)
    {
        if(habitacion == 6 && llaveVista)
        {
            infoText.Text = "No puedo robar la llave mientras él esté aquí."; 
        }
        else if (habitacion == 3 && libroVisto)
        {
            infoText.Text = "Recoges el libro de recetas. Pesa más de lo que esperabas. Prefieres no pensar en por qué.";
            habLibro = -1;
            itemJugador = 1;
            ActualizarVista();
        }
        else
        {
            infoText.Text = "No hay nada que recoger. Tal vez deberías probar a observar.";
        }
    }    
    void btnUsar_Clicked(object sender, EventArgs e)
    {
        if (habitacion == 9 && itemJugador == 1)
        {
            infoText.Text = "Le tiendes el libro al cocinero. Lo hojea frenéticamente hasta encontrar la página. Sus ojos brillan de una forma que te incomoda. \"¡Por fin! ¡Ahora sí podremos comer!\" No preguntas qué lleva la receta.";
            habLibro = -1;
            itemJugador = -1;
            comidaHecha = true;
        }
        else
        {
            infoText.Text = "No tengo nada útil que usar aquí.";
        }
    }
}