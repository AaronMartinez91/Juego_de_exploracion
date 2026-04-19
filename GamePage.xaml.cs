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
                infoText.Text = "Tu tío sonríe. \"Ah, la llavecita...\" La tapa con la mano sin dejar de mirarte. \"No es nada importante.\" Hace una pausa. \"Oye, ¿no tendrás algo con qué escribir? No encuentro ni un bolígrafo en esta casa.\"";
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
                infoText.Text = "Cocinero: Llevo horas intentando recordar el ingrediente especial de esta receta... Sin él no puedo terminar el plato. Y sin el plato... mejor ni pensarlo.";
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
        else
        {
            infoText.Text = "No hay nadie en la habitación";
        }
    }    
    void btnRecoger_Clicked(object sender, EventArgs e)
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
        else
        {
            infoText.Text = "No hay nada que recoger. Tal vez deberías probar a observar.";
        }
    }    
    void btnUsar_Clicked(object sender, EventArgs e)
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
        else
        {
            infoText.Text = "No tengo nada útil que usar aquí.";
        }
    }
}