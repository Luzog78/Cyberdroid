<div align="center">
  
  # Semaine 5
  
  <br>
  
  ## En plein dans les formules...

  > GitHub supporte le LateX donc c'est cool !

  ### Objectif : Créer notre propre système de gravitation.
  
</div>

<br>

- Notre jeu sera à base de réflexion et c'est pas mal de pouvoir jouer avec la gravité !

<br>

- Il nous fallait nous décider :

  - Changer la direction de la gravité quand on entre dans une zone par exemple *(donc utiliser la __Physique__ générale)*.

  - Coder son propre système de gravitation (qui pourrait donc être multiple).<br>
  Mais dans ce cas, comment procéder :

    - Définir une classe **`Attracteur`** avec les variables suivantes : *`forceAttraction` (nombre flottant)* ;

    - Ou définir une classe **`ZoneAttraction`** avec les variables suivantes : *`forceAttraction` (nombre flottant)*, *`directionAttraction` (vecteur tridimentionnel)*.

    > Dans les deux cas, il fallait une classe **`Attractif`** qui subirait (calculerait) toutes les interactions gravitationnelles.

<br>

- Mais en essayant de bidouiller les valeurs du module **Physics** de Unity, on s'est vite rendu compte qu'il n'était pas possible de gérer *"plusieurs gravités"*.

<br>

- Et le fait d'avoir un *"point d'attraction"* est en réalité un peu contraignant dans notre situation : le sol est bien droit et bien horizontal donc on veut pouvoir par défaut être attiré vers le bas et non vers le centre de la pièce.
> **Remarque :** *Même si cette idée n'est pas du tout à écarter pour d'autres types de projets.*<br>
**Exemple :** *Sur un jeu où notre avatar devrait se déplacer sur plusieurs planètes (__Super Mario Galaxy__ par exemple), cette solution est la meilleure de très loin.*

<br><br>

---

<br><br>

- Donc pour créer nos *"zones d'attraction"*, il nous fallait nous renseigner sur... **Comment simuler la gravité ?**

<br>

- Mais en réalité, après plusieurs temps de reflexion... le principe est très simple :

<div align="center">

  $${\huge \text{Chaque element qui subit la gravite a une masse } m \text{.}}$$

  $${\huge \text{Nous connaissons egalement le vecteur constant de gravitation } \vec{\Gamma} \text{.}}$$

  $${\huge \text{Unity possede, pour reguler les calculs, une variable de temps } \delta \text{.}}$$

  <br>

  $${\huge \text{L'acceleration que subit un objet est donc :}}$$

  $${\huge \vec{\alpha} = m \times \vec{\Gamma} \times \delta}$$

</div>

<br>

- On a donc commencé le travail :

> **Remarque :** *Toutes les classes affichées sont évidemment incomplètes, ne laissant que les bouts de code "intéressants".*

`GravityMotor.cs` :
```csharp
public bool activated = true;
// Liste des zones dans lesquelles l'entité se trouve.
public List<GravityArea> gravityAreas = new List<GravityArea>();

private Rigidbody rb;

[SerializeField] Vector3 Gravity
{
    get
    {
        if (gravityAreas.Count == 0)
            return Vector3.zero;
        GravityArea area = gravityAreas.Last();
        return area.gravityDirection * area.gravityStrength;
    }
}

void FixedUpdate()
{
    if (activated)
    {
        rb.AddForce(Gravity * rb.mass * Time.deltaTime, ForceMode.Acceleration);
    }
}
```

`GravityArea.cs` :
```csharp
public bool activated = true;
public float gravityStrength = 900f;
public Vector3 gravityDirection = Vector3.down;

// Quand une entité entre dans la zone.
void OnTriggerEnter(Collider other)
{
    if(other.TryGetComponent(out GravityMotor motor))
    {
        if(activated)
            motor.gravityAreas.Add(this);
        else
            motor.gravityAreas.Remove(this);
    }
}

// Quand une entité sort dans la zone.
void OnTriggerExit(Collider other)
{
    if (other.TryGetComponent(out GravityMotor motor))
    {
        motor.gravityAreas.Remove(this);
    }
}
```

<br>

- Ça a fonctionné pendant... ${\large \approx 10 \text { secondes.}}$

- Le problème était que le joueur ... tombait. Il se retrouvait après quelques secondes sur le flanc.

- Impossible d'avancer ni même de rester sans bouger.

- On a dû trouver une solution : (dans `GravityMotor.cs`)
```csharp
void Start()
{
    rb = GetComponent<Rigidbody>();
    rb.constraints = RigidbodyConstraints.FreezeRotation;
    /*
     * Avec cette contrainte, normalement, il est impossible
     *  "naturellement" de changer sa rotation.
     * Seul un script peut mettre à jour la rotation.
     *  --> C'est exactement ce qu'on veut !
     */
}
```

<br>

- Mais cette fois-ci, quand on changeait de zone de gravitation.. on restait figés dans la même direction.<br>
On était donc *"allongé"* sur le *"nouveau sol"*. Et ça, c'est un problème.

- On avait donc plus qu'à trouver un moyen de changer notre direction pour rester toujours vertical **par rapport à notre gravité actuelle $\vec{\Gamma}$**.

- Ça sonnait simple mais en réalité, ça ne l'était pas du tout.

- On a essayé beaucoup de choses :

  - Figer la rotation $y$ du perso,
  > Mais dans ce cas, le joueur ne peut pas s'adapter à son environnement.

  - Définir la rotation $y$ à l'inverse de la coordonnée $y$ du vecteur gravité $\vec{\Gamma}$
  > Mais si ça n'est pas la position $y$ qui est non-nulle dans $\vec{\Gamma}$...

  - Ajouter la rotation actuelle à l'inverse du vecteur $\vec{\Gamma}$
  > **Mais comment faire ça...**

<br>

- Pour étudier la rotation d'un objet dans l'espace, on trouve 2 grands types de moyens pour calculer des rotations : les __Quaternions__ ainsi que les __Angles d'Euler__.

  - Les __Angles d'Euler__ sont un *triplet* de *nombres flottants*, avec des exprimé en *degrés* donc ça semblait formidable. Mais les mesures sont __arbitraires__, c'est-à dire qu'elles peuvent changer, en fonction de la rotation et de la position dans le monde, de référentiel.

  - Tandis que les [__Quaternions__](https://fr.wikipedia.org/wiki/Quaternion) sont une branche des nombres imaginaires exprimés sous la forme : ${\large a + bi + cj + dk, \left ( a ; b ; c ; d \right ) \in \mathbb{R}^{4}}$ qui représente les rotations de manière bien plus complexes à comprendre mais également bien plus simple à manipuler, à bases d'opérations élémentaires.

### C'est ici qu'a commencé l'étude des Quaternions

- En effet, même si par la suite, toutes les recherches effectuées ne seront pas utilisées à leur plein potentiel dans le cadre de ce problème, elles ont permis la découverte et la compréhension de très beaux et de très ingénieux principes mathématiques.
> Un exemple est juste le principe de multiplication : les formules de multiplications des quaternions sont très proches des multiplications matricielles et permettent une parfaite manipulation des coordonnées, vecteurs et rotations dans l'espace.

<br>

- Enfin bref, après cette découverte formidable, la question nous paraissait d'une simplicité monstrueuse !

- Les trois lignes de code requises nous ont paru d'un naturel sans égale : (dans `GravityMotor.cs`)
```csharp
public bool activated = true;

//...

    // Dans le FixedUpdate()
    if (rotate)
    {
        Quaternion rotation = Quaternion.FromToRotation(transform.up, -gravity) * transform.rotation;
        rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
        transform.rotation = rotation;
    }

// ...
```

<br>

- Notre problème était enfin résolu !

<br>

- Un dernier subsistait encore : **La superposition des Zones**.

- Qu'on a réglé avec un simple système de priorité :

`GravityZone.cs` :
```csharp
public int priority = 0;
// ...
```
`GravityMotor.cs` :
```csharp
Vector3 Gravity
{
    get
    {
        // ...
        gravityAreas.Sort((area1, area2) => area1.priority.CompareTo(area2.priority));
        // ...
    }
}
```

<br><br>

- Et voilà. **Tout notre système de gravité est opérationnel !!**

- Quelques screenshots de la *TestRoom*.

![img](/resources/0362.png?raw=true)

![img](/resources/9329.png?raw=true)

<br>

<div align="right">
  <br><br>

  ---

  A la semaine pro ! 😉
  > — &nbsp;&nbsp; _Luzog78_ &nbsp;&&nbsp; _notpolarstar_
</div>
