<div align="center">
  
  # Semaine 2
  
  <br>
  
  ## Débuts avec l'IDE

  > Une découverte un peu difficile.
  > Mais au moins, on finira bilingue !
  
</div>

<br>

- Durant cette semaine, la plupart de nos soirées se résumait à voir encore et encore de multiples tutoriels sur l'utilisation de *Unity*.

- Un point positif : on remarque vite que le niveau d'anglais s'améliore énormément et super vite !

- Donc après première ouverture de Unity, ce qu'on constate :
  - L'interface semble beaucoup trop complexe,
  - Le bouton "Play" ne fonctionne pas,
  - Des problèmes dans la console,
  - Impossible de se déplacer dans l'environnement.
  > Bref tout ça fait qu'on en a déjà marre. Après à peine quelques minutes.

![img](/resources/1373.png?raw=true)

- Mais on prend notre courage à deux mains et on avance.

<br>

- La routine commence à s'installer :
  1. Analyser une erreur de la console,
  2. Tenter de le résoudre,
  3. Ne pas y arriver donc chercher sur les forums,
  > Merci d'ailleurs aux contributeurs de [Stack Overflow](https://stackoverflow.com/) et du [Unity Forum](https://forum.unity.com/) sur lesquels nous avons passé plusieurs dizaines d'heures.
  4. Enfin trouver une solution miraculeuse (après une ou deux bonnes heures),
  5. Et recommencer !

<br>

- Durant ces longs moments de test et de fails, nous avons appris les bases du logiciel :

  - Comment créer un objet,

  - Comment fonctionne l'architecture,

  - Qu'est-ce qu'un objet,
  > **Petit a parte :** *En vérité, c'est très ingénieux : un objet, quel qu'il soit, n'est qu'une boîte vide dans laquelle on ajoute les différents composants dont on a besoin.*<br>
  *Chaque objet __DOIT__ posséder le composant __TRANSFORM__ qui indique une __position__, une __roation__ et une __echelle__ de taille.*<br>
  *Maintenant, si je veux pourvoir __voir__ mon objet, il lui faut un composant __MESH__ : "mesh" c'est la surface sur laquelle va être affichée une texture.*<br>
  *De plus, si je veux un objet qui peut interagir avec l'environnement, il faut un composant __COLLIDER__ afin de savoir quand 2 objets entrent en contact (le plus utilisé étant le __MESH COLLIDER__ qui crée une collision à partir de la surface de l'objet)*<br>
  *Et enfin, si on veut créer un solide, avec de la physique (pour qu'il puisse "tomber" par exemple), il lui faut ajouter le composant __RIGID BODY__ qui gère la masse de l'objet, sa vélocité etc.*

  - Comment manipuler la caméra

  - Comment se déplacer dans l'éditeur pour mieux comprendre et se situer dans l'environnement.

  - etc.

![img](/resources/3955.png?raw=true)

<br>

- Tout ce processus nous a pris la semaine entière et nous ne sommes même pas à 1% de l'entière compréhension du logiciel.

<br>

- Objectif pour la semaine prochaine : créer un joueur, pouvoir le déplacer et gérer.... La ***gravité*** ........ *(explications dans le prochain épisode)*

<div align="right">
  <br><br>

  ---

  A la semaine pro ! 😉
  > — &nbsp;&nbsp; _Luzog78_ &nbsp;&&nbsp; _notpolarstar_
</div>
