namespace B2ACT2Juego_de_exploracion;
using Plugin.Maui.Audio;

public partial class GamePage : ContentPage
{
    // Audio
    IAudioPlayer playerAmbiente;
    IAudioPlayer playerRonquido;
    IAudioPlayer playerMaullido;
    IAudioPlayer playerPajaro;
    IAudioPlayer playerLlave;
    IAudioPlayer playerPuerta;

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
    bool gatoVisto = false;
    bool pajaroVisto = false;
    bool tiaVista = false;
    bool habladoCocinero = false;
    bool comidaHecha = false;
    bool tioEscribiendo = false;
    bool puertaCamaraVista = false;
    bool puertaCamaraAbierta = false;
    bool gatoUsado = false;
    bool tioHabladoLibro = false;
    bool habladoTioPluma = false;
    bool camaraObservada = false;
    bool camaraRecogerIntentado = false;
    bool porcheMostrado = false;

    public GamePage()
	{
        InitializeComponent();

        // La carga de partida se ejecuta al crear la página
        string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
        if (File.Exists(ruta))
        {
            CargarPartida();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (playerAmbiente == null)
        {
            playerAmbiente = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("creeping_ambience.mp3"));
            playerAmbiente.Volume = 0.5;
            playerAmbiente.Loop = true;
            playerAmbiente.Play();
        }

        ActualizarVista();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        playerAmbiente?.Stop();
        GuardarPartida();
    }

    async void ActualizarVista()
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
        imgItemJugador.Source = "swap_bag.png";

        // Mostrar texto de dónde se encuentra el jugador. (Texto especial introductorio + actualización en el porche)
        if (habitacion == 1 && !porcheMostrado)
        {
            infoText.Text = "Llevas diez minutos parado frente a la mansión sin moverte. Tus tíos te llamaron hace tres días. 'Ven a visitarnos', dijeron. Sin más explicación. Siempre han sido así... Amables en la superficie, con esa sonrisa que dura un segundo de más. Sabes que ocultan algo aquí dentro. Siempre lo has sabido. Respiras hondo y empujas la puerta.";
            porcheMostrado = true;
        }
        else if (habitacion == 1)
        {
            infoText.Text = "Vuelves al porche. Algo te dice que deberías irte, pero algo más fuerte te atrapa a seguir aquí.";
        }
        else
        {
            infoText.Text = "Entras en " + nombresHabitacion[habitacion] + ".";
        }

        // mostrar personaje en las habitaciones
        if (habTio == habitacion)
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
        }
        if(itemJugador == 1)
        {
            imgItemJugador.Source = "i04_libro.png";
        }
        if(itemJugador == 2)
        {
            imgItemJugador.Source = "i02_gato.png";
        }
        if(itemJugador == 3)
        {
            imgItemJugador.Source = "i03_pluma.png";
        }

        // Sonidos contextuales
        if (habitacion == 10 && objetivo == 1)
        {
            if (playerRonquido == null)
            {
                playerRonquido = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("snoring.mp3"));
                playerRonquido.Volume = 1.0;
                playerRonquido.Loop = true;
                playerRonquido.Play();
            }
        }
        else
        {
            playerRonquido?.Stop();
            playerRonquido = null;
        }

        if (habitacion == 8 && objetivo == 2 && !gatoUsado && itemJugador != 2 && habladoTioPluma)
        {
            if (playerMaullido == null)
            {
                playerMaullido = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("meow.mp3"));
                playerMaullido.Loop = true;
                playerMaullido.Volume = 1.0;
                playerMaullido.Play();
            }
        }
        else
        {
            playerMaullido?.Stop();
            playerMaullido = null;
        }

        if (habitacion == 12 && objetivo == 2 && !gatoUsado)
        {
            if (playerPajaro == null)
            {
                playerPajaro = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("birdSound.wav"));
                playerPajaro.Loop = true;
                playerPajaro.Volume = 1.0;
                playerPajaro.Play();
            }
        }
        else
        {
            playerPajaro?.Stop();
            playerPajaro = null;
        }

    }

    // Botones de direcciones

    void btnNorte_Clicked(object sender, EventArgs e)
    {
        habitacion = mapa[habitacion][0];
        ActualizarVista();
        GuardarPartida();
    }

    void btnSur_Clicked(object sender, EventArgs e)
    {
        habitacion = mapa[habitacion][1];
        ActualizarVista();
        GuardarPartida();
    }

    async void btnEste_Clicked(object sender, EventArgs e)
    {
        if(habitacion == 10 && objetivo == 1 && !comidaHecha && !tiaVista)
        {
            infoText.Text = "Tu tía sigue durmiendo al otro lado. Un sueño profundo, casi antinatural. En tu familia siempre dijeron que solo había dos cosas capaces de arrancarla de la cama: un incendio y la hora de comer. Rezas para que sea la segunda.";
            tiaVista = true;
        }
        else if (habitacion == 10 && objetivo == 1 && !comidaHecha && tiaVista)
        {
            infoText.Text = "Tu tía sigue durmiendo. No va a salir por su propia voluntad.";
        }
        else if (habitacion == 10 && objetivo == 1 && comidaHecha)
        {
            infoText.Text = "Oyes un revuelo al otro lado de la puerta. Algo se mueve ahí dentro. Quizás deberías observar antes de entrar.";
        }
        else if (habitacion == 7 && (!camaraObservada || !camaraRecogerIntentado))
        {
            infoText.Text = "Algo te dice que deberías echar un vistazo antes de marcharte.";
        }
        else if (habitacion == 7 && camaraObservada && camaraRecogerIntentado)
        {
            await Shell.Current.GoToAsync("End");
        }
        else
        {
            habitacion = mapa[habitacion][2];
            ActualizarVista();
        }
        GuardarPartida();
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
        GuardarPartida();
    }

    // Botones de acciones
    void btnObservar_Clicked(object sender, EventArgs e)
    {
        // quito la llave en el primer caso porque es redundante y no es realmente necesaria hasta el 3er objetivo

        //if(habitacion == 6 && objetivo == 1)
        //{
        //    infoText.Text = "Observas una llave cerca de tu tío. Está demasiado atento como para poder cogerla.";
        //    llaveVista = true;
        //    habLlave = 6;
        //    imgItemHabitacion.Source = "i01_llave.png";
        //    imgItemHabitacion.IsVisible = true;
        //}
        if (habitacion == 6 && objetivo == 2 && !llaveVista)
        {
            infoText.Text = "Mientras echas un vistazo por la habitación reparas en una llave cerca de tu tío. Al darte cuenta de que la estás mirando, él levanta la vista y te sostiene la mirada un instante de más. Sonríe. Apartas los ojos.";
            llaveVista = true;
            habLlave = 6;
            imgItemHabitacion.Source = "i01_llave.png";
            imgItemHabitacion.IsVisible = true;
        }
        else if (habitacion == 6 && objetivo == 2 && llaveVista && habLlave == 6 && !tioEscribiendo)
        {
            infoText.Text = "La llave sigue ahí. Si consiguiera distraerlo lo suficiente...";
        }
        else if (habitacion == 6 && objetivo == 2 && llaveVista && habLlave == 6 && tioEscribiendo)
        {
            infoText.Text = "El tío está absorto escribiendo. La llave está al alcance de tu mano.";
        }
        else if(habitacion == 5 && !puertaCamaraVista)
        {
            infoText.Text = "Observas una puerta oculta... ¿Podría ser una cámara secreta?";
            puertaCamaraVista = true;
            btnOeste.IsVisible = true;
        }
        else if (habitacion == 10 && !comidaHecha && objetivo == 1 && !tiaVista)
        {
            infoText.Text = "Acercas el ojo a la cerradura. Un error que lamentarás el resto de tu vida. Tu tía se remueve entre las sábanas y el pijama no deja demasiado a la imaginación. Cierras los ojos, pero ya es demasiado tarde. Algunas imágenes no se pueden desver.";
        }
        else if (habitacion == 10 && !comidaHecha && objetivo == 1 && tiaVista)
        {
            infoText.Text = "Algo dentro de ti considera volver a mirar. Esa parte de ti lleva tiempo dándote problemas.";
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
        else if (habitacion == 3 && !libroVisto && habladoCocinero && tioHabladoLibro)
        {
            infoText.Text = "Ves un libro de recetas sobre la mesa. Qué cosa tan mundana en una mansión tan... peculiar.";
            libroVisto = true;
            habLibro = 3;
            imgItemHabitacion.Source = "i04_libro.png";
            imgItemHabitacion.IsVisible = true;
        }
        else if (habitacion == 3 && !libroVisto && habladoCocinero && !tioHabladoLibro)
        {
            infoText.Text = "Hay demasiados libros aquí. El cocinero no recordaba el título exacto y tú tampoco sabes por dónde empezar. Quizás alguien de la casa sepa algo más.";
        }
        else if (habitacion == 8 && objetivo == 2 && !gatoVisto && habladoTioPluma)
        {
            infoText.Text = "Hay un gato bajo la silla de tu tía, inmóvil, con los ojos clavados en el plato. O es muy valiente o muy estúpido. Probablemente lo segundo.";
            gatoVisto = true;
            habGato = 8;
            imgItemHabitacion.Source = "i02_gato.png";
            imgItemHabitacion.IsVisible = true;
        }
        else if (habitacion == 12 && !pajaroVisto)
        {
            infoText.Text = "Posado en la barandilla hay un pájaro gordo y confiado que no parece tener ninguna prisa por irse. Una presa fácil para cualquier depredador con las agallas suficientes.";
            pajaroVisto = true;
        }
        else if (habitacion == 12 && pajaroVisto && !habladoTioPluma && !gatoUsado)
        {
            infoText.Text = "El pájaro sigue en la barandilla. No parece tener ninguna prisa por irse.";
        }
        else if (habitacion == 12 && pajaroVisto && itemJugador == 2 && !gatoUsado)
        {
            infoText.Text = "El pájaro sigue ahí. El gato lleva un rato mirándolo fijamente desde tus brazos. Sabes lo que tienes que hacer.";
        }
        else if (habitacion == 12 && pajaroVisto && habPluma == 12 && gatoUsado)
        {
            infoText.Text = "Solo queda una pluma en el suelo. Deberías recogerla.";
        }
        else if (habitacion == 12 && pajaroVisto && itemJugador != 2 && habPluma == -1 && itemJugador != 3 && gatoUsado)
        {
            infoText.Text = "El balcón está vacío. Una extraña calma inunda el lugar.";
        }
        else if (habitacion == 7 && !camaraObservada)
        {
            infoText.Text = "La habitación huele a algo que no consigues identificar. Las estanterías están llenas de objetos cubiertos de polvo y telarañas. Cajones cerrados con llave. Frascos con etiquetas escritas a mano que prefieres no leer. En el centro, una vieja silla desvencijada.";
            camaraObservada = true;
        }
        else if (habitacion == 7 && camaraObservada)
        {
            infoText.Text = "No quieres seguir aquí. Ya has visto suficiente.";
        }
        else if (habitacion == 11 && objetivo == 2)
        {
            infoText.Text = "Entras despacio. El olor te golpea antes que nada. Las sábanas están retorcidas y hay manchas oscuras en el colchón que prefieres no examinar de cerca. Algo pegajoso en el suelo brilla levemente bajo la luz. Hay cosas que es mejor no investigar.";
        }
        else if (habitacion == 13)
        {
            infoText.Text = "Todo parece normal a primera vista. Demasiado normal. El jabón está perfectamente colocado, las toallas dobladas con una precisión casi quirúrgica. Entonces ves el espejo. Alguien ha escrito algo en el vapor con el dedo. Las letras ya casi se han borrado pero puedes leer: 'sal'. No sabes cuándo fue escrito.";
        }
        else
        {
            infoText.Text = "No observas nada relevante";
        }
    }    
    void btnHablar_Clicked(object sender, EventArgs e)
    {
        if (habitacion == 6 && objetivo == 1)
        {
            if(!habladoCocinero)
            {
                infoText.Text = "Tu tío te saluda efusivamente, su contacto excesivo te pone un poco nervioso, pero no le das importancia.";
            }
            else
            {
                infoText.Text = "Tu tío entorna los ojos. \"¿Un libro de recetas? Creo que vi uno hace poco... en el salón, sobre la mesa. No tiene pérdida.\" Lo dice con demasiada naturalidad para alguien que supuestamente no sabía nada.";
                tioHabladoLibro = true;
            }
        }
        else if (habitacion == 6 && objetivo == 2)
        {
            if (!llaveVista)
            {
                infoText.Text = "Tu tío está absorto en sus pensamientos, tamborileando los dedos sobre el escritorio. No repara en ti.";
            }
            else if(!tioEscribiendo)
            {
                infoText.Text = "Tu tío sonríe. \"Ah, la llavecita...\" La tapa con la mano sin dejar de mirarte. \"No es nada importante.\" Hace una pausa. \"Oye, ¿no tendrás algo con qué escribir? No encuentro ni una triste pluma en esta casa.\"";
                habladoTioPluma = true;
            }
            else
            {
                infoText.Text = "Tu tío escribe concentrado, inclinado sobre el papel. No levanta la vista. Sus hombros se mueven de forma rítmica. Prefieres no saber qué está escribiendo.";
            }
        }
        else if (habitacion == 6 && objetivo == 3)
        {
            infoText.Text = "Tu tío sigue escribiendo, completamente ajeno a todo. No levanta la vista. Mejor así.";
        }
        else if (habitacion == 9 && objetivo == 1)
        {
            if (!tiaVista)
            {
                infoText.Text = "El cocinero está atareado entre los fogones. Apenas levanta la vista para reconocer tu presencia. Parece concentrado en lo suyo.";
            }
            else if (!comidaHecha)
            {
                infoText.Text = "Cocinero: Llevo horas intentando recordar el ingrediente especial de esta receta... Sin él no puedo terminar el plato. Y sin el plato... mejor ni pensarlo. ¿Dónde habré dejado ese maldito libro de recetas 'especiales' del señor?";
                habladoCocinero = true;
            }
            else
            {
                infoText.Text = "El cocinero está concentrado en los fogones. Se le ve con una energía renovada, casi inquietante. \"¡Ya casi está!\" exclama sin girarse. No preguntas qué es exactamente lo que casi está.";
            }
        }
        else if (habitacion == 9 && objetivo == 2)
        {
            infoText.Text = "El cocinero observa desde la puerta cómo tu tía devora el plato. Tiene los ojos húmedos. \"Come bien\", murmura con voz temblorosa. No sabes si es de alivio o de miedo. Probablemente las dos cosas.";
        }
        else if (habitacion == 8 && objetivo == 2)
        {
            infoText.Text = "Tu tía levanta la vista del plato por un instante. Tiene algo en la comisura de los labios que prefieres no identificar. Emite un sonido gutural que interpretas como un saludo y vuelve a hundirse en el plato. El tenedor apenas toca la comida.";
        }
        else if (habitacion == 7)
        {
            infoText.Text = "No hay nadie aquí. Por suerte.";
        }
        else
        {
            infoText.Text = "No hay nadie en la habitación";
        }
    }    
    async void btnRecoger_Clicked(object sender, EventArgs e)
    {
        if(habitacion == 6 && llaveVista && !tioEscribiendo && habLlave == 6)
        {
            infoText.Text = "No puedo robar la llave mientras él esté aquí."; 
        }
        else if(habitacion == 6 && llaveVista && tioEscribiendo && habLlave == 6)
        {
            habLlave = -1;
            itemJugador = 0;
            objetivo = 3;
            playerLlave = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("key_pickup.mp3"));
            playerLlave.Play();
            ActualizarVista();
            infoText.Text = "Con tu tío absorto en el papel, deslizas la llave hacia ti sin hacer ruido. La guardas rápido. Ni siquiera levanta la vista.";
        }
        else if (habitacion == 3 && libroVisto && habLibro == 3)
        {
            habLibro = -1;
            itemJugador = 1;
            ActualizarVista();
            infoText.Text = "Recoges el libro de recetas. Pesa más de lo que esperabas. Prefieres no pensar en por qué.";
        }
        else if (habitacion == 8 && gatoVisto && habGato == 8)
        {
            habGato = -1;
            itemJugador = 2;
            ActualizarVista();
            infoText.Text = "Lo coges. Está en los huesos. Te mira con unos ojos que llevan días sin ver comida de verdad. Entiendes que la desesperación lo trajo hasta aquí. Tal vez podrías encontrarle algo que llevarse a la boca.";
        }
        else if (habitacion == 12 && habPluma == 12)
        {
            habPluma = -1;
            itemJugador = 3;
            ActualizarVista();
            infoText.Text = "Coges la pluma del suelo. Es lo único que queda de lo que acaba de pasar aquí.";
        }
        else if (habitacion == 12 && pajaroVisto && habPluma != 12 && itemJugador != 3 && habladoTioPluma)
        {
            infoText.Text = "No puedes recoger al pájaro así como así. Necesitas algo que lo distraiga.";
        }
        else if (habitacion == 12 && itemJugador == 2 && pajaroVisto)
        {
            infoText.Text = "El gato se retuerce entre tus brazos mirando al pájaro fijamente. Tal vez debería soltarlo.";
        }
        else if (habitacion == 12 && itemJugador == 2 && !pajaroVisto)
        {
            infoText.Text = "El gato se retuerce inquieto entre tus brazos. Algo en esta habitación le llama la atención. Quizás deberías observar.";
        }
        else if (habitacion == 7 && !camaraRecogerIntentado)
        {
            infoText.Text = "Extiendes la mano hacia uno de los objetos y la retiras de inmediato. Hay cosas que una vez tocas no puedes dejar de sentir. Decides que esto no es para ti.";
            camaraRecogerIntentado = true;
        }
        else if (habitacion == 7 && camaraRecogerIntentado)
        {
            infoText.Text = "Con una vez es más que suficiente. No vas a repetirlo.";
        }
        else
        {
            infoText.Text = "No hay nada que recoger. Tal vez deberías probar a observar.";
        }
    }    
    async void btnUsar_Clicked(object sender, EventArgs e)
    {
        if (habitacion == 9 && itemJugador == 1)
        {
            habLibro = -1;
            itemJugador = -1;
            comidaHecha = true;
            ActualizarVista();
            infoText.Text = "Le tiendes el libro al cocinero. Lo hojea frenéticamente hasta encontrar la página. Sus ojos brillan de una forma que te incomoda. \"¡Por fin! ¡Ahora sí podremos comer!\" No preguntas qué lleva la receta.";
        }
        else if (habitacion == 12 && itemJugador == 2 && pajaroVisto)
        {
            itemJugador = -1;
            habGato = -1;
            habPluma = 12;
            gatoUsado = true;
            ActualizarVista();
            infoText.Text = "Sueltas al gato. Tarda exactamente medio segundo en localizar al pájaro. Lo que ocurre después es rápido, caótico y ruidoso. Cuando el silencio vuelve, no queda ni rastro de ninguno de los dos. Solo una pluma flotando en el aire.";
        }
        else if (habitacion == 6 && itemJugador == 3 && objetivo == 2)
        {
            tioEscribiendo = true;
            itemJugador = -1;
            habPluma = -1;
            ActualizarVista();
            infoText.Text = "Le tiendes la pluma a tu tío. La coge sin decir nada, con una sonrisa que no llega a sus ojos. Se inclina sobre el papel y empieza a escribir. No quieres saber qué.";
        }
        else if (habitacion == 5 && itemJugador == 0 && objetivo == 3 && puertaCamaraVista)
        {
            puertaCamaraAbierta = true;
            itemJugador = -1;
            playerPuerta = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("creaky_door.wav"));
            playerPuerta.Play();
            ActualizarVista();
            infoText.Text = "Introduces la llave en la cerradura. Un clic seco. La puerta cede. Al otro lado, oscuridad.";
        }
        else if (habitacion == 5 && itemJugador == 0 && objetivo == 3 && !puertaCamaraVista)
        {
            infoText.Text = "Tienes una llave pero no sabes dónde encaja. Quizás deberías observar mejor esta habitación.";
        }
        else if (habitacion == 7)
        {
            infoText.Text = "No. Rotundamente no.";
        }
        else
        {
            infoText.Text = "No tengo nada útil que usar aquí.";
        }
    }

    // Sistema de guardado
    void GuardarPartida()
    {
        string linea = $"{habitacion},{objetivo},{itemJugador}," +
                       $"{habTio},{habTia},{habCocinero}," +
                       $"{habLlave},{habLibro},{habGato},{habPluma}," +
                       $"{(libroVisto ? 1 : 0)},{(llaveVista ? 1 : 0)},{(gatoVisto ? 1 : 0)},{(pajaroVisto ? 1 : 0)}," +
                       $"{(tiaVista ? 1 : 0)},{(habladoCocinero ? 1 : 0)},{(comidaHecha ? 1 : 0)},{(tioEscribiendo ? 1 : 0)}," +
                       $"{(puertaCamaraVista ? 1 : 0)},{(puertaCamaraAbierta ? 1 : 0)},{(gatoUsado ? 1 : 0)}," +
                       $"{(tioHabladoLibro ? 1 : 0)},{(habladoTioPluma ? 1 : 0)}," +
                       $"{(camaraObservada ? 1 : 0)},{(camaraRecogerIntentado ? 1 : 0)}," +
                       $"{(porcheMostrado ? 1 : 0)}";

        string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
        FileStream stream = new FileStream(ruta, FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        writer.WriteLine(linea);
        writer.Close();
        stream.Close();
    }

    // Sistema de carga
    void CargarPartida()
    {
        string ruta = System.IO.Path.Combine(FileSystem.AppDataDirectory, "partida.txt");
        FileStream stream = new FileStream(ruta, FileMode.Open);
        StreamReader reader = new StreamReader(stream);
        string linea = reader.ReadLine();
        reader.Close();
        stream.Close();

        string[] datos = linea.Split(',');

        habitacion = int.Parse(datos[0]);
        objetivo = int.Parse(datos[1]);
        itemJugador = int.Parse(datos[2]);
        habTio = int.Parse(datos[3]);
        habTia = int.Parse(datos[4]);
        habCocinero = int.Parse(datos[5]);
        habLlave = int.Parse(datos[6]);
        habLibro = int.Parse(datos[7]);
        habGato = int.Parse(datos[8]);
        habPluma = int.Parse(datos[9]);

        if (datos[10] == "1")
        {
            libroVisto = true;
        }
        else
        {
            libroVisto = false;
        }

        if (datos[11] == "1")
        {
            llaveVista = true;
        }
        else
        {
            llaveVista = false;
        }

        if (datos[12] == "1")
        {
            gatoVisto = true;
        }
        else
        {
            gatoVisto = false;
        }

        if (datos[13] == "1")
        {
            pajaroVisto = true;
        }
        else
        {
            pajaroVisto = false;
        }

        if (datos[14] == "1")
        {
            tiaVista = true;
        }
        else
        {
            tiaVista = false;
        }

        if (datos[15] == "1")
        {
            habladoCocinero = true;
        }
        else
        {
            habladoCocinero = false;
        }

        if (datos[16] == "1")
        {
            comidaHecha = true;
        }
        else
        {
            comidaHecha = false;
        }

        if (datos[17] == "1")
        {
            tioEscribiendo = true;
        }
        else
        {
            tioEscribiendo = false;
        }

        if (datos[18] == "1")
        {
            puertaCamaraVista = true;
        }
        else
        {
            puertaCamaraVista = false;
        }

        if (datos[19] == "1")
        {
            puertaCamaraAbierta = true;
        }
        else
        {
            puertaCamaraAbierta = false;
        }

        if (datos[20] == "1")
        {
            gatoUsado = true;
        }
        else
        {
            gatoUsado = false;
        }

        if (datos[21] == "1")
        {
            tioHabladoLibro = true;
        }
        else
        {
            tioHabladoLibro = false;
        }

        if (datos[22] == "1")
        {
            habladoTioPluma = true;
        }
        else
        {
            habladoTioPluma = false;
        }

        if (datos[23] == "1")
        {
            camaraObservada = true;
        }
        else
        {
            camaraObservada = false;
        }

        if (datos[24] == "1")
        {
            camaraRecogerIntentado = true;
        }
        else
        {
            camaraRecogerIntentado = false;
        }

        if (datos[25] == "1")
        {
            porcheMostrado = true;
        }
        else
        {
            porcheMostrado = false;
        }
    }
}