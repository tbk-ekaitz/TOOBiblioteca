
### Leyenda de Estado

-   **[NUEVO]**: Funcionalidad nueva extra.
-   **[MAS]**: Funcionalidad solicitada, pero implementada con extras.
-   **[CAMBIO UX]**: Diferencia en la interfaz de usuario respecto al boceto original añadiendo a lo pedido **(MAS, pero en los Forms)**.

Disclaimer:
No fui apuntando que cosas extra ponia, fui añadiendolos segun lo que sentia que era necesario al momento. Mala mia, y horrible forma de desplanificar el desarrollo, pero asi fue. 
Si hay algo fuera de lo pedido, y no esta anotado aca, disculpas por ello.

----------

### 1. Arquitectura y Datos

-   [x] **Persistencia Volátil con Datos de Prueba**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** No especificado.
    -   **Implementación:** Clase `Repositorio` que precarga datos de prueba al iniciar para que la app no esté vacía. Interna a capa de negocio. Inicialmente queria añadirlo en una 4ta capa, esta en Biblioteca.Negocio.Persistencia.
    - **CREDENCIALES**
			Para probar todo, al abrir el Form de login aparece escrito por defecto las credenciales del Empleado admin, que tiene acceso a todo.
Para revisar los roles, si ya estas dentro cerrar sesion, y estos son:

			admin: "12345678A", "admin123"
			sala: "23456789B" , "sala123"
			adq: "34567890C", "adq123"
        

### 2. Gestión de Usuarios

-   [x] **Datos de Usuario Ampliados**
    -   **Estado:** **[MAS]**
    -   **Requisito:** Se pedía solo "Nombre y DNI".
    -   **Implementación:** Se añadieron: Apellidos, Email, Teléfono, Dirección, Fecha Alta y Límite de Préstamos (Max 5).
        
-   [x] **Unificación de Listados y Detalle (`FormListados`)**
    -   **Estado:** **[CAMBIO UX]**
    -   **Requisito:** `P13TOO` pedía dos formularios separados: "Listado ordenado" (dos ListBox) y "Recorrido uno a uno".
    -   **Implementación:** Un solo formulario avanzado con `SplitContainer`: lista filtrable a la izquierda y edición/detalle a la derecha sincronizados.
        
-   [x] **Bloqueo de Baja por Integridad**
    -   **Estado:** **[MAS]**
    -   **Requisito:** `P12TOO` pedía flujo de baja simple con confirmación.
    -   **Implementación:** Se impide borrar un usuario si tiene préstamos activos (regla de integridad).
        

### 3. Gestión de Documentos y Ejemplares
        
-   [x] **Atributos Específicos de Documentos**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** Básicos (Título, Autor, ISBN).
    -   **Implementación:** Añadidos: Nº Páginas, Encuadernación (Libros); Narrador, Nº Discos (Audiolibros).
        
-   [x] **Enums Estrictos para Formatos**
    -   **Estado:** **[MAS]**
    -   **Requisito:** Se sugería texto libre "mp3, aac...".
    -   **Implementación:** Enum cerrado (`MP3, WAV, AAC, FLAC...`) para evitar errores de datos.
        
-   [x] **Autogeneración de Códigos de Ejemplar**
    -   **Estado:** **[CAMBIO UX]**
    -   **Requisito:** `P12TOO` mostraba un campo para escribir manualmente "EJ10".
    -   **Implementación:** El sistema genera códigos automáticos (`ISBN-01`, `ISBN-02`) asegurando unicidad.
        

### 4. Gestión de Préstamos

-   [x] **Sanciones Automáticas**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** Se pedía "saber si tiene algo fuera de plazo".
    -   **Implementación:** Lógica activa que aplica `Días Sanción = Días Retraso * 2` y bloquea nuevos préstamos. Es posible crear y desactivar sanciones.
        
-   [x] **Funcionalidad de "Renovar"**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** No solicitado.
    -   **Implementación:** Permite extender fecha si no hay sanción vigente.
        
-   [x] **Estado "Cancelado"**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** Solo "En proceso" y "Finalizado".
    -   **Implementación:** Permite anular préstamos erróneos sin considerarlos devueltos. O cancelar préstamos vencidos sin tener que penalizar.
        
-   [x] **Alta de Préstamo sin Ventana Emergente**
    -   **Estado:** **[CAMBIO UX]**
    -   **Requisito:** `P13TOO` pedía un botón que abriera otra ventana para seleccionar el ejemplar.
    -   **Implementación:** Entrada directa de código de barras en el mismo formulario + Tecla `Enter` para añadir rápido (simulación pistola lectora).
        
-   [x] **Acceso Directo a Crear (+) y Borrar Visual (X)**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** Uso genérico de `BindingNavigator`.
    -   **Implementación:** Botón `+` reprogramado para abrir `FormAltaPrestamo`; Botón `X` configurado para limpieza visual de la lista.
        

### 5. Interfaz General y Seguridad

- [x] **Gestión de Instancias por Contexto (Singleton por Tab)**
	-   **Estado:** **[CAMBIO UX]**
	-   **Implementación:** "Documentos" y "Estadísticas" son el mismo formulario (`FormDocumentos`) pero se abren en pestañas distintas según el botón del menú. El sistema controla las instancias para permitir **una ventana por cada tab** simultáneamente, pero evita duplicados de la misma vista.

-   [x] **Login con Detección Automática de Rol**
    -   **Estado:** **[MAS]**
    -   **Requisito:** `P12TOO` mostraba RadioButtons para que el usuario eligiera "Sala" o "Adquisiciones".
    -   **Implementación:** El sistema deduce el rol según las credenciales (más seguro). Se añade rol "Administrador". Facil cerrar sesion y abrir con otra cuenta.
        
-   [x] **Dashboard de Estadísticas**
    -   **Estado:** **[MAS]**
    -   **Requisito:** Se pedía un dato simple ("Documento más leído del mes").
    -   **Implementación:** Panel gráfico completo con Top 5, histórico y distribuciones (por rellenar facil el form que estaba muy vacio), aparte de documento mas leido del mes (el que aparece en grande es el historico).
        
-   [x] **Barra de Estado en Tiempo Real (Timer)**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** No solicitado.
    -   **Implementación:** Recálculo cada segundo de inventario (Total/Prestados) en la barra inferior (fue para hacerme la vida mas sencilla al probarlo, pero me termino gustando como quedaba).
        
-   [x] **Validación con `ErrorProvider`**
    -   **Estado:** **[CAMBIO UX]**
    -   **Requisito:** Uso de `MessageBox` para errores.
    -   **Implementación:** Iconos parpadeantes en los campos erróneos (menos intrusivo).
        
-   [x] **Feedback Visual**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** No solicitado.
    -   **Implementación:** Colores en tablas según disponibilidad / gravedad del retraso / eliminado pero guardado en historico.

-   [x] **Interoperabilidad de los forms**
    -   **Estado:** **[NUEVO]**
    -   **Requisito:** No solicitado.
    -   **Implementación:** Permite abrir otros forms de mostrar informacion / editar con doble click en los elementos de las tablas. Y a su vez dentro de estos elementos tambien se puede cancelar o renovar prestamos, pero para devolver un prestamo, no. Es a proposito. No replicado en la lista de vencidos, al ser un subset de funcionalidad del listado de prestamos (solo sirve para visualizar vencidos rapido, sin seleccionar el filtrado en listado de prestamos).