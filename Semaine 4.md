<div align="center">
  
  # Semaine 4
  
  <br>
  
  ## Premiers pas !

  > On avance assez bien !
  > Même si la physique c'est pas facile.

  ### Objectif : Créer un joueur contrôlable.
  
</div>

<br>

- Pour cette étape, rien de plus simple *(lol)* :

  <br>

  - Pour le corps, on choisit l'objet __capsule__.
  > C'est l'objet primitif qui ressemble le plus à un corps.

  <br><br>

  - Ensuite, il faut ajouter une petite flopée de __colliders__ afin de gérer les collisions de notre joueur.
  > C'est effectivement bien mieux s'il ne traverse pas les murs.

    - Le __collider principal__, qui englobe au mieux le personnage.<br>
    Il gère les collisions avec les sols et toutes les autres collisions (non spécifiées plus bas).

    - Le __collider mural__, qui est plus petit en hauteur mais très légèrement bombé afin de gérer les collisions avec les murs.
    > En effet, on a besoin d'un autre collider pour les murs car la physique de Unity fait en sorte que par défaut, si notre vélocité augmente contre un mur par exemple, elle va juste être mise à zéro : si le joueur le fait en sautant, il vole.<br>
    C'est l'effet de "friction" qui influe sur cette partie. On applique donc un filtre permettant de retirer toute friction à notre collider mural.<br>
    **Attention :** il ne faut pas le faire sur le collider principal car sinon, impossible de s'arrêter : les frictions avec le sol n'existent plus.

    - Les __colliders de biais__, utilisés pour détecter les collisions quand le sol est en biais, situés en bas du personnage.
    > Quand le joueur, en jouant avec la gravité, se retrouve à marcher sur 2 murs, il subit beaucoup trop de frictions : il ne peut plus avancer.<br>
    On applique donc à ces colliers le filtre permettant de retirer toutes les frictions.

  <br><br>

  - Et enfin, on définit un point d'accroche qui servira plus tard pour la caméra.

  <br><br>

![img](/resources/2288.png?raw=true)
![img](/resources/4730.png?raw=true)
![img](/resources/4535.png?raw=true)

  <br><br>

- On a finalement terminé la partie graphique de notre personnage.

---

- Maintenant, il faut pouvoir le contrôler. Alors c'est parti pour apprendre le `C#` et les modules de bases de Unity.

<br>

  - Tout d'abords les variables utiles à notre joueur :
```c
public bool activated = true;
public float speed = 10f, runningSpeed = 15f, turnSpeed = 200f;
public float jump = 6f;

public GameObject cameraAnchor;

private Rigidbody rb;
```

  - Ensuite les fonctions de base :

```c
void Start()
{
    rb = GetComponent<Rigidbody>();
}
```
```c
void Update()
{
    if (activated) {
        if (Input.GetKey(KeyCode.Space)) {
            rb.AddForce(rb.transform.up * jump, ForceMode.Impulse);
        }

        float finalSpeed = Input.GetKey(KeyCode.F) ? runningSpeed : speed;

        if (Input.GetKey(KeyCode.S)) {
            rb.AddForce(rb.transform.forward * finalSpeed, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.W)) {
            rb.AddForce(-rb.transform.forward * finalSpeed, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.Q)) {
            rb.AddForce(-rb.transform.right * finalSpeed, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.D)) {
            rb.AddForce(rb.transform.right * finalSpeed, ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}
```

  - Premier problème rencontré : le joueur n'a pas de limite de vitesse ! C'est un problème très facilement solvable par une simple condition :
```c
void Update()
{
        //...
        if(rb.velocity.magnitude > finalSpeed) {
            rb.velocity = rb.velocity.normalized * finalSpeed;
        }
    }
}
```

  - Et tout fonctionnait très bien mais là. **Terrible problème !** Quand on appuie sur `[SPACE]`, on saute beaucoup beaucoup trop haut.

  - Après de longue minutes de réflexion, c'est évident, il faut un **cooldown** afin d'éviter de sauter 1 000 000 de fois en 0.1 sec :
```c
public bool isJumping;

IEnumerator JumpCooldown()
{
    isJumping = true;
    yield return new WaitForSeconds(0.1f);
    isJumping = false;
}

void Update()
{
        //...
        if (Input.GetKey(KeyCode.Space) && !isJumping) {
            rb.AddForce(rb.transform.up * jump, ForceMode.Impulse);
            isJumping = true;
            StartCoroutine(JumpCooldown());
        }
        //...
}
```

  - Et voilà un problème de réglé !!<br>
  Mais s'il n'y en avait qu'un, ça serait trop facile...

  - Le deuxième problème maintenant, c'est que si je tombe, je peux tout de même sauter... Alors il suffit simplement d'ajouter une dernière variable.
  > **Explications :** Pour pallier à ce problème, on va *imaginer* tracer un rayon (un __Raycast__), et on va analyser ce que ce rayon traverse. La fonction __Raycast__ renvoie un *true* si le rayon traverse un __collider__ *(de niveau 1 ou plus, ce qui évite les collisions avec les colliders du joueur ou encore les matières comme l'eau)*. C'est exactement ce que fait la ligne 5.
```c
public bool isGrounded;
public float groundDistance = 1.1f;

void CheckGrounded()
{
    isGrounded = Physics.Raycast(rb.transform.position, -rb.transform.up, groundDistance, 1);
}

void Update()
{
        //...
        CheckGrounded();
        if (Input.GetKey(KeyCode.Space) && isGrounded && !isJumping) {
            rb.AddForce(rb.transform.up * jump, ForceMode.Impulse);
            isJumping = true;
            StartCoroutine(JumpCooldown());
        }
        //...
}
```

  - Maintenant que notre personnage fonctionne parfaitement, on ajoute une dernière chose, le fait de pouvoir contrôler la caméra avec la souris, un geste bien plus naturel pour les joueurs (au lieu de devoir utiliser le clavier pour effectuer des rotations), tout en cachant le curseur, en le bloquant au centre de l'écran, et en imposant des limites verticales (pour ne pas retourner la caméra).
  > **Petite aparté :** Ce passage a été très intéressant et riche car calculer une rotation dans un univers tridimensionnel est très intéressant d'un point de vue mathématique.<br>
  En effet, on trouve 2 grands types de moyens pour calculer des rotations : les __Quaternions__ ainsi que les __Angles d'Euler__. Nous avons donc dû faire de nombreuses recherches pour comprendre ces systèmes et pour tenter de déterminer lequel était plus facilement manipulable dans ce contexte.<br>
  Quand les quaternions sont infiniment trop pratiques pour ajouter, soustraire et multiplier des rotations, les angles d'Euler sont quant à eux bien plus faciles à utiliser quand on souhaite définir ou calculer les bornes d'une rotation. Ils sont donc utilisés dans ce cas.
```c
public Vector2 mouseSensitivity = new Vector2(150f, 150f);

void Start() {
    // ...
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

void Update()
{
        //...
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseSensitivity.y * Time.deltaTime);
        if(cameraAnchor != null) {
            cameraAnchor.transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * mouseSensitivity.x * Time.deltaTime);
            float rot = cameraAnchor.transform.localEulerAngles.x;
            if (rot > 60 && rot < 290)
            {
                // 60 >= x° >= 0 || 360 >= x° >= 290
                bool tooLow = Mathf.Abs(rot - 60) < Mathf.Abs(rot - 290);
                cameraAnchor.transform.localEulerAngles = new Vector3(tooLow ? 60 : 290, 0, 0);
            }
        }
    }
}
```

- Et voilà un super personnage 100% jouable !

<br><br>

- Objectif la semaine prochaine : Créer un système de gravitation.


<div align="right">
  <br><br>

  ---

  A la semaine pro ! 😉
  > — &nbsp;&nbsp; _Luzog78_ &nbsp;&&nbsp; _notpolarstar_
</div>



