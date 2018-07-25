/*Modificamos algunos valores por defecto*/

Alter table preguntas modify column Correcta longtext NULL DEFAULT NULL;

Alter table respuestas modify column Notas varchar(4000) NULL DEFAULT NULL;
Alter table respuestas modify column NotasAdmin varchar(4000) NULL DEFAULT NULL;
Alter table respuestas modify column Estado Int(11) NULL DEFAULT '0';

Alter table notassections modify column Notas varchar(4000) NULL DEFAULT NULL;
Alter table notasasignaciones modify column Notas varchar(4000) NULL DEFAULT NULL;

Alter table evaluaciones modify column NotasEvaluacion varchar(4000) NULL DEFAULT NULL;
Alter table evaluaciones modify column NotasObjetivos varchar(4000) NULL DEFAULT NULL;
Alter table evaluaciones modify column Puntuacion double NULL DEFAULT '0';

Alter table asignaciones modify column Peso INT(11) NULL DEFAULT '0';


############################################
/*Añadimos algunos datos a la tabla (proyectos, usuarios, etc)*/
INSERT INTO `agilemeter`.`users` (`Nombre`,`Password`) VALUES
('Admin','c1c224b03cd9bc7b6a86d77f5dace40191766c485cd55dc48caf9ac873335d6f'),
('Test','532eaabd9574880dbf76b9b8cc00832c20a6ec113d682299550d7a6e0f345e25'),
('User','b512d97e7cbf97c273e4db073bbb547aa65a84589227f8f3d9e4a72b9372a24d');


INSERT INTO `agilemeter`.`sections` (`Id`,`Nombre`) VALUES
(1,'CEREMONIAS'),
(2,'ROLES'),
(3,'ARTEFACTOS');

INSERT INTO `agilemeter`.`roles` (`Id`,`Role`) VALUES
(1,'Usuario'),
(2,'Administrador');


INSERT INTO `agilemeter`.`user_roles` (`Id`,`RoleId`,`UserNombre`) VALUES
(1,1,'User'),
(2,1,'Admin'),
(3,1,'Admin');

INSERT INTO `agilemeter`.`proyectos` (`Id`,`Fecha`,`Nombre`,`UserNombre`) VALUES
(1,'2018-07-10 00:00:00','BCA','Admin'),
(2,'2018-07-10 00:00:00','TESCO','Admin'),
(3,'2018-07-10 00:00:00','BestDay','User'),
(4,'2018-07-10 00:00:00','TVE','User'),
(5,'2018-07-10 00:00:00','Proyecto Test','Test');


############################################
/*ASIGNACIONES*/

INSERT INTO `asignaciones`(`Id`, `Nombre`, `SectionId`, `Peso`) VALUES (1, "Daily", 1, 5),
(2, "Retrospective", 1, 30), (3, "Sprint Review", 1, 20), (4, "Sprint Planning", 1, 15), (5, "Refinement", 1, 10);

INSERT INTO `asignaciones`(`Id`, `Nombre`, `SectionId`, `Peso`) VALUES (6, "Product Owner", 2, 20),
(7, "Scrum Master", 2, 60), (8, "Equipo Desarrollo", 2, 20);

INSERT INTO `asignaciones`(`Id`, `Nombre`, `SectionId`, `Peso`) VALUES (9, "Product Backlog", 3, 30),
(10, "Sprint Backlog", 3, 20), (11, "Incremento", 3, 15), (12, "Iteracion", 3, 5), (13, "Metricas", 3, 10);


############################################
/*PREGUNTAS*/

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 1 , '¿Se realiza la daily?', NULL),
( 1 , '¿El equipo completo participa?', 'Si'),
( 1 , '¿Se emplean como máximo 15 min?', 'Si'),
( 1 , '¿Se mencionan los problemas e impedimentos?', 'Si'),
( 1 , '¿Se revisan en cada daily los objetivos del Sprint?', 'Si'),
( 1 , '¿Se realiza siempre a la misma hora y lugar?', 'Si'),
( 1 , '¿Participa gente que no pertenece al equipo?', 'No');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 2 , '¿Se realiza la Retrospective al final de cada sprint?', NULL),
( 2 , '¿Se plantean propuestas SMART?', 'Si'),
( 2 , '¿Se implementan las propuestas?', 'Si'),
( 2 , '¿Equipo al completo más PO participan?', 'Si'),
( 2 , '¿Se analizan los problemas en profundidad?', 'Si'),
( 2 , '¿Participa gente que no pertenece al equipo?', 'No'),
( 2 , '¿Todo el equipo expresa su punto de vista?', 'Si'),
( 2 , '¿Se analizan las métricas y su impacto durante la retro?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 3 , '¿Se realiza la Sprint Review al final de cada sprint?', NULL),
( 3 , '¿Se muestra software funcionando y probado?', 'Si'),
( 3 , '¿Se recibe feedback de interesados y PO?', 'Si'),
( 3 , '¿Se mencionan los items inacabados?', 'No'),
( 3 , '¿Se revisa si se ha alcanzado el objetivo del Sprint?', 'Si'),
( 3 , '¿Se muestran los items acabados al 99%?', 'No');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 4 , '¿Se realiza Sprint Planning por cada Sprint?', NULL),
( 4 , '¿El PO está disponible para dudas?', 'Si'),
( 4 , '¿Está el PB preparado para el Sprint Planning?', 'Si'),
( 4 , '¿El equipo completo participa?', 'Si'),
( 4 , '¿El resultado de la sesión es el plan del Sprint?', 'Si'),
( 4 , '¿El equipo completo cree que el plan es alcanzable?', 'Si'),
( 4 , '¿El PO queda satisfecho con las prioridades?', 'Si'),
( 4 , '¿Los PBI se dividen en tareas?', 'Si'),
( 4 , '¿Las tareas son estimadas?', 'Si'),
( 4 , '¿Se adquiere un compromiso por parte del equipo?', 'Si'); 

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 5 , '¿Se realiza Refinement?', NULL),
( 5 , '¿Es el PO quien decide cuando se hace un refinement?', 'Si'),
( 5 , '¿El PO lleva las US definidas para discutir?', 'Si'),
( 5 , '¿Se estima en tamaño relativo?', 'Si'),
( 5 , '¿Existe DoR?', 'Si'),
( 5 , '¿Se aplica DoR?', 'Si'),
( 5 , '¿Se realizan preguntas y propuestas?', 'Si'),
( 5 , '¿Participa todo el equipo?', 'Si'),
( 5 , '¿Participa en la estimación personas ajenas al equipo?', 'No');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 6 , '¿Existe el rol de PO en el equipo?', NULL),
( 6 , '¿El PO tiene poder para priorizar los elementos del PB?', 'Si'),
( 6 , '¿El PO tiene el conocimiento suficiente para priorizar?', 'Si'),
( 6 , '¿El PO tiene contacto directo con el equipo?', 'Si'),
( 6 , '¿El PO tiene contacto directo con los interesados?', 'Si'),
( 6 , '¿El PO tiene voz única (Si es equipo, solo hay una opinión)?', 'Si'),
( 6 , '¿El PO tiene la visión del producto?', 'Si'),
( 6 , '¿El PO hace otras labores (codificar por ejemplo)?', 'No'),
( 6 , '¿El PO toma decisiones técnicas?', 'No');


INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 7, '¿Existe el rol de SM en el equipo?', NULL),
( 7, '¿El SM se sienta con el equipo?', 'Si'),
( 7, '¿El SM se enfoca en la resolución de impedimentos?', 'Si'),
( 7, '¿El SM escala los impedimentos?', 'Si'),
( 7, '¿El SM hace otras labores (codificar/analizar por ejemplo)?', 'No'),
( 7, '¿El SM toma decisiones técnicas o de negocio?', 'No'),
( 7, '¿El SM ayuda/guía al PO para realizar correctamente su trabajo?', 'Si'),
( 7, '¿El SM empodera al equipo?', 'Si'),
( 7, '¿El SM asume la responsabilidad si el equipo falla?', 'No'),
( 7, '¿El SM permite que el equipo experimente y se equivoque?', 'Si'),
( 7, '¿Los líderes o managers de la organización conocen y/o comparten los principios ágiles?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 8 , '¿El equipo tiene todas las habilidades necesarias?', 'Si'),
( 8 , '¿Existen miembros del equipo encasillados, no conociendo absolutamente nada de otras áreas?', 'Si'),
( 8 , '¿Los miembros del equipo se sientan juntos?', 'Si'),
( 8 , '¿Hay com máximo 9 personas por equipo?', 'Si'),
( 8 , '¿Hay algún miembro del equipo que odie Scrum?', 'No'),
( 8 , '¿Hay algún miembro del equipo profundamente desmotivado?', 'No'),
( 8 , '¿Tiene el equipo un drag factor interiorizado, planificado y consensuado con los stakeholders?', 'Si'),
( 8 , '¿Se realizan reuniones adicionales que estén fuera del framework de Scrum?', 'No'),
( 8 , '¿El equipo usa o dispone de herramientas para organizar sus tareas?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 9 , '¿Existe PB?', NULL),
( 9 , '¿EL PB es visible y refleja la visión del producto?', 'Si'),
( 9 , '¿Los PBI se priorizan por su valor de negocio?', 'Si'),
( 9 , '¿Los PBI se estiman?', 'Si'),
( 9 , '¿El equipo completo es quien realiza las estimaciones?', 'Si'),
( 9 , '¿Los PBI son tan pequeños como para abordarse en un Sprint?', 'Si'),
( 9 , '¿El PO entiende el propósito de todos los PBI?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 10 , '¿Existe SB?', NULL),
( 10 , '¿El SB es visible y refleja el compromiso para el Sprint?', 'Si'),
( 10 , '¿El SB se actualiza diariamente?', 'Si'),
( 10 , '¿El SB es propiedad exclusiva del equipo?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 11 , '¿Existe DoD?', NULL),
( 11 , '¿El DoD es alcanzable dentro de cada iteración?', 'Si'),
( 11 , '¿El equipo respeta el DoD?', 'Si'),
( 11 , '¿El Software entregado tiene calidad para subirse a producción si el negocio así lo pidiera?', 'Si'),
( 11 , '¿Se actualiza el DoD?', 'Si'),
( 11 , '¿Tanto PO como equipo están de acuerdo con el DoD?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 12 , '¿Existe iteraciones de tiempo fijo?', NULL),
( 12 , '¿La longitud de las iteraciones está entre 2-4 semanas?', 'Si'),
( 12 , '¿Siempre terminan a tiempo?', 'Si'),
( 12 , '¿El equipo es interrumpido durante una iteración?', 'No'),
( 12 , '¿El equipo normalmente entrega lo que comprometió?', 'Si'),
( 12 , '¿Se ha cancelado alguna iteración que ha sido un fracaso?', 'Si');

INSERT INTO `preguntas`(`AsignacionId`, `Pregunta`, `Correcta`) VALUES
( 13 , '¿Se mide la velodidad del equipo?', NULL),
( 13 , '¿Todos los PBI se estiman y se computan en la velocidad?', 'Si'),
( 13 , '¿El PO usa la velocidad para planificar a futuro?', 'Si'),
( 13 , '¿La velocidad sólo incluye PBI terminados?', 'Si'),
( 13 , '¿El equipo tiene un Burndown por Sprint?', 'Si'),
( 13 , '¿El Burndown es visible por todos los miembros del equipo?', 'Si'),
( 13 , '¿El Burndown se actualiza diariamente?', 'Si'),
( 13 , '¿El equipo conoce y entiende sus métricas?', 'Si');

