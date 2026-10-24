# ridigProject — Plataforma Interativa 2D (Unity 6)

## 1. Contexto Acadêmico
- **Projeto:** ridigProject
- **Curso:** Bacharelado em Ciência da Computação
- **Disciplina:** Desenvolvimento de Jogos Digitais
- **Atividade:** Implementação de Plataforma Interativa com Movimento Horizontal e Queda por Gravidade
- **Motor de Jogo:** Unity 6 (6000.5.9f1)
- **Linguagem:** C# (.NET Standard 2.0)

---

## 2. Descrição Geral do Projeto
Este projeto consiste em um protótipo de jogo de plataforma 2D desenvolvido em Unity 6. O cenário conta com estética pixel art, física bidimensional realista (`Rigidbody2D` e `Collider2D`), câmera ortográfica com acompanhamento suave e uma plataforma mecânica interativa (`MovingFallingPlatform`).

A mecânica central desenvolvida atende aos requisitos da atividade: a plataforma interativa permanece estática até que o personagem faça contato físico com ela. Ao ser tocada, desloca-se horizontalmente para a direita durante exatamente 5 segundos e, em seguida, interrompe a velocidade horizontal e cai sob a ação da gravidade.

---

## 3. Estados da Plataforma Interativa (`MovingFallingPlatform`)

O ciclo de vida da plataforma é gerenciado por uma máquina de estados finitos (`enum PlatformState`):

```
       [Contato Físico do Player]                 [Após 5 Segundos]
[IDLE] --------------------------> [MOVING] --------------------------> [FALLING]
(Imóvel, GravityScale=0)          (linearVelocity=(v,0), Dur=5s)       (linearVelocity=0, GravityScale=2)
```

1. **Estado 1 — Idle (Repouso):**
   - A plataforma permanece completamente imóvel no início da execução.
   - `Rigidbody2D.bodyType = Dynamic` com `gravityScale = 0`.
   - `RigidbodyConstraints2D.FreezeRotation` ativo (impede rotações indesejadas no eixo Z).
   - `linearVelocity = Vector2.zero`.
   - Aguarda o primeiro contato físico através de `OnCollisionEnter2D`.

2. **Estado 2 — Moving (Movimentação Horizontal):**
   - Ativado imediatamente após o primeiro contato com um objeto identificado com a tag `Player`.
   - A plataforma se desloca horizontalmente para a direita com velocidade constante (`moveSpeed = 3f`).
   - A movimentação utiliza a API nativa da Unity 6: `rb.linearVelocity = new Vector2(moveSpeed, 0f)`.
   - Permanece sem influência da gravidade (`gravityScale = 0`).
   - O tempo de deslocamento é controlado com precisão de física através de uma corrotina sincronizada com `WaitForFixedUpdate` e `Time.fixedDeltaTime`, totalizando exatamente 5 segundos de tempo de jogo.
   - Novas colisões durante este período são ignoradas, prevenindo reinicializações do contador ou alterações de velocidade.

3. **Estado 3 — Falling (Queda por Gravidade):**
   - Transição automática após decorridos os 5 segundos.
   - A velocidade horizontal é imediatamente zerada (`linearVelocity = Vector2.zero`), eliminando qualquer momento residual.
   - A gravidade é ativada via física 2D (`rb.gravityScale = fallGravityScale`, configurável no Inspector).
   - A plataforma cai naturalmente através da simulação física da Unity, sem interpolações manuais ou scripts de translação.
   - A transição é irreversível, garantindo ativação única por instância da plataforma.

---

## 4. Controles do Personagem

O projeto utiliza o **New Input System** da Unity:

| Ação | Teclas | Descrição |
| :--- | :--- | :--- |
| **Mover para a Esquerda** | `A` ou `Seta para a Esquerda` | Desloca o personagem para a esquerda |
| **Mover para a Direita** | `D` ou `Seta para a Direita` | Desloca o personagem para a direita |
| **Pular** | `Barra de Espaço` | Executa o salto vertical se o personagem estiver no solo |

O controlador do jogador (`PlayerController2D`) compensa automaticamente o deslocamento quando sobre a plataforma móvel, permitindo que o personagem acompanhe suavemente o movimento horizontal sem escorregar.

---

## 5. Estrutura de Arquivos e Componentes

### Scripts (`Assets/Scripts/`)
- `MovingFallingPlatform.cs`: Componente principal que implementa os três estados da plataforma interativa, a detecção física com `OnCollisionEnter2D`, o temporizador e a ativação da gravidade.
- `PlayerController2D.cs`: Controlador do jogador com movimentação horizontal, salto, verificação de solo e acompanhamento de velocidade da plataforma.
- `CameraFollow2D.cs`: Suavização da câmera ortográfica acompanhando o jogador através de `Vector3.SmoothDamp`.

### Sprites (`Assets/Sprites/`)
- `Player.png`: Sprite pixel art do personagem (24x32 px, Point Filter, 16 PPU).
- `StartingPlatform.png`: Sprite de plataforma de início em estilo grama/terra (64x16 px).
- `MovingPlatform.png`: Sprite estilizado metálico industrial com sinalização visual direcional (64x16 px).
- `PlatformPhysicsMaterial.physicsMaterial2D`: Material físico com atrito adequado para evitar instabilidades.

### Configuração do GameObject `MovingFallingPlatform` na Cena
- **Transform:** Posição inicial acessível `(3, 0.5, 0)` à direita da plataforma inicial `(-5, -0.5, 0)`.
- **SpriteRenderer:** Sprite `MovingPlatform` com sorting order apropriada.
- **BoxCollider2D:** Tamanho `(4, 1)`, `isTrigger = false`.
- **Rigidbody2D:** `bodyType = Dynamic`, `gravityScale = 0`, `collisionDetectionMode = Continuous`, `interpolation = Interpolate`, `constraints = FreezeRotation`.
- **MovingFallingPlatform:**
  - `Move Speed`: `3`
  - `Movement Duration`: `5`
  - `Fall Gravity Scale`: `2`
  - `Player Tag`: `"Player"`

---

## 6. Instruções para Executar o Projeto

1. Abra o projeto no **Unity Editor (versão 6000.5.9f1 ou compatível)**.
2. Na aba **Project**, abra a cena principal localizada em:
   `Assets/Scenes/SampleScene.unity`
3. Pressione o botão **Play** no topo do editor da Unity.
4. Utilize `A` / `D` para movimentar o personagem e `Espaço` para saltar sobre a plataforma `MovingFallingPlatform`.
5. Observe a sequência:
   - A plataforma permanece imóvel até o toque.
   - Ao tocar, desloca-se para a direita durante 5 segundos.
   - Ao completar 5 segundos, para o movimento horizontal e cai naturalmente.

---

## 7. Versionamento Git
O repositório está configurado seguindo os padrões recomendados para Unity, mantendo apenas os diretórios versionáveis:
- `Assets/`
- `Packages/`
- `ProjectSettings/`

Pastas temporárias e de cache local como `Library/`, `Temp/`, `Obj/` e `Logs/` são ignoradas.
