SimpleRBM
=========

Restricted Boltzmann Machine and Deep Belief Network Implementation in C#
based primarily on the papers:<BR> "<a href="http://www.iro.umontreal.ca/~bengioy/papers/ftml.pdf">Learning Deep Architectures for AI</a>" By Joshua Bengio
<BR>And "<a href="http://www.cs.toronto.edu/~hinton/absps/guideTR.pdf">A Practical Guide to Training Restricted Boltzmann Machines</a>" By Geoffrey Hinton<BR>
<BR>
<B>RBM.cs</b> - Contains the core code for the Restricted boltzmann machine. The number of lines of code have been minimized to mimic a python implementation.
This gives us clean and readable code.<BR>
<b>DeepBeliefNetwork.cs</b> - Is a simple multilayer RBM where the hidden layer is connected to a lower level RBM's visible layer.<BR>

<b>Matrix.cs and Vector.cs</b> Classes that help in simplifying the code.<BR>
<BR>
<i>A note about Multidimentional arrays in C# (<=4.0)</i> - These arrays are the most natural way to do matrix multiplications and operations.<BR>
However, due to a poor CLR implementation, they work much slower than traditional jagged arrays. Hence, we use Jagged arrays in our Matric.cs implementation, and apply a simple [x,y] indexer to mimic a multidimentional array.


<b>Example:</b>
The project consists of a simple console mode example, where we give our AI many images of handwritten digits ranging from 0 to 9.<BR>
We the try to reconstruct several images (encoding and decoding) and we get a relative good approximation of the source.<BR>
The next step in the program is "Daydreaming" where we imput random samples and reconstruction yields various random images that depict strong features learned by the machine.
