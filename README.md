# Jogo de Plataforma 2D
O rigidProject é um protótipo de jogo de plataforma 2D desenvolvido na Unity 6, utilizando C# e recursos de física bidimensional. O projeto foi elaborado como atividade prática da disciplina de Desenvolvimento de Jogos Digitais, com o objetivo de explorar conceitos de movimentação, detecção de colisões, interação entre objetos e aplicação de forças físicas em ambientes de jogos digitais.

O cenário apresenta uma estética pixel art, câmera ortográfica com acompanhamento suave do personagem e um sistema de movimentação baseado no New Input System da Unity. O jogador pode deslocar-se horizontalmente utilizando as teclas A/D ou as setas direcionais e executar saltos com a barra de espaço.

A principal funcionalidade implementada é uma plataforma mecânica interativa que permanece imóvel até detectar o contato físico com o personagem. Após a colisão, a plataforma inicia automaticamente um deslocamento horizontal para a direita durante 5 segundos. Ao término desse período, sua velocidade horizontal é interrompida e a plataforma passa a cair naturalmente sob a ação da gravidade.

A implementação utiliza componentes Rigidbody2D e Collider2D para simulação física e detecção de colisões, além de scripts em C# responsáveis pelo controle do personagem e pelo comportamento da plataforma. O sistema também permite que o jogador acompanhe suavemente o deslocamento da plataforma enquanto permanece sobre ela, evitando deslizamentos indesejados.
