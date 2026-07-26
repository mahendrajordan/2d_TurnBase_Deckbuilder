# Dungeon Adventure

# Game Overview
game petualang turn base deck builder, diawal melakukan setup kartu yg akan digunakan untuk melawan setiap musuh yg di temukan dalam dungen.
kalah kan semua musuh di setian level untuk memenangkan permainan, pemain kalah ketika player mati.
build deck kartu kalin sebaik mungkin agar bisa bertahan hingga akhir

# How to Run
- Engine & version used:Unity Engine with editor version 6000.0.75f1
- Build location: https://magtimus.itch.io/dungeon-adventure
- How to Run : export file > run 'Dungeon_Adventure.exe'
 
# Technical Decisions
perancangan awal
konsep awal dimana pemain melam kumpulan monster,
pemain memiliki deck dengan 5 kartu di tangan, setiap kartu punya cost nya masing2 dengan limit cost di awal adalah 2.

target awal adalah pemain dapat memilih kartu untuk menyerang musuh, jika sudah cost sudah tercapai maka end turn dan gilirian musuh jalan jika musuh sudah selesai jalan, maka masuk round baru dan di mulai dari player lagi.
jika pemain kalah maka akan balik ke main menu.
jika pemain menang akan masuk ke stage berikut nya, sebelum masuk player akan memilih satu dari 3 kartu baru.

di dalam game terdiri 4 status utama, yaitu :
healt -> nyawa character
attack roll -> akurasi serangan dengan formula 1d20 + attack roll
damage roll -> damage bonus yg diberikan character
armor class -> kemampuan memblokir/menghindar serangan 

di game ini akurasi dan damage berdasrkan putaran dadu, dengan aturan 2d6 -> berarti 2 dadu dengan masing2 dadu bernilai maksimal 6 dengan output range (2-12)

target utama saat ini adalah bagaiman sistem deck bekerja, efek kartu yg bisa di berikan yaitu memberi damage, efek buff dan debuff, dan juga dot.

# What I Would Do With More Time
- untuk kedepan direncanakan setiap lanjut ke stage berikut nya pemain akan memilih bonus stats tambahan, 
- membuatan jenis efek skill baru seperti yg simple increase die roll setiap 2 turn, atau disable jenis kartu tertentu,
- sistem class/ character yg bisa memiliki sinergi dengan kartu tertentu, seperti Barbarian dengan trait reckless stance -> ketika menggunakan Rage attack roll +1, wizard dengan trait untraining hand -> kartu selain magic punya pinalty attack roll -1 tapi kartu bertipe magic memiliki bonus attack roll +1 

# Known Issues
- ketika select card ada kemungkinan kecil ketika di klik lagi malah menjalan kan action nya, ini sudah aku coba perbaiki di raycast kursor nya tapi belum yakin apakah itu dapat menyelsaikan masalah ini, untuk saat ini belum menemukan masalah ini lagi
- move card, ketika kartu di select lalu balik ketangan urutan nya berubah, ini karena untuk sekarang kartu hanyak balik ke transform parent nya tapi tidak ke susunan child nya
- magic missile, efek kartu untuk saat ini hanya berefek ke kartu di tangan saja tidak di kartu di deck. misal pakai 1 kali magic missile harusnya kartu berikut nya memiliki dmg roll 1d5 tapi jika kartu di ambil dari deck balik ke dmg roll awal 1d2
- efek ubh stats terkadang suka stuck, dimana value nya ke double setelah recovery. bagian ini sudah di perbaiki, tapi mungkin masih ada kemungkinan kecil muncul kembali

# Core Script
# DeckBuilderMaster.cs
tempat system deck nya, dimana menyimpan list card yg akan dipakai, memanggil card ke tangan, dan end turn untuk menyelesaikan turn nya.

DrawCardOnHand(int cardAmount = 5, bool isResetUseCard = true)
memanggil card ke tangan, di sini ada aturan jumlah kartu yg dipanggil, juga mengecek batas kartu yg sama (untuk normal maksimal 3 kartu yg sama), disini juga ada CanUseThisCard(CardData cardData) yg digunakan untuk melimit kartu dalam 1 turn. misal kartu 'draw2card' hanya bisa dipanggil 3 kali dalam 1 turn.

MoveCardTo(Transform obj, Transform parentTarget, int delayMultiple, float startScale = 0, float endScale = 1)
sistem untuk menjalan kan animasi untuk memindahkan kartu dari deck ke hand atau dari hand ke trash

ActiveCard(MainBody mainBody)
tempat untuk mengakses 'Card.cs' untuk menjalan kan perintah kartu, seperti menyerang, memberi buff atau debuff, atau special skill effect.
setelah itu kartu ini di buang ke trash

# Card.cs
tempat system card nya berjalan, seperti melakukan serangan, memberi buff atau debuff, atau special skill effect.

Setup(CardData _cardData, MainBody _mainBody, Transform handTransform, Transform trashTransform, Transform offTransform)
diakses lewat 'DeckBuilderMaster.cs' untuk mensetup data yg diperlukan kartu.

SelectCard()
untuk mengecek ketika di klick apakah card akan select atau unselect

GetSelectType(bool isSelect)
untuk mengecek ketika select siapa yg akan menjadi target, sperti jika card type = damage maka akan menarget musuh, card type = buff maka akan menarget kan player

ActionCard(MainBody target)
menjalankan aksi card nya, akan mengecek terlebih dahulu jenis CardType nya untuk menentukan aksi nya. apakah menyerang, menyerang dan memberi debuff, dan lain sebagai nya

SetupSpecialEffec()
ini khusu kartu bertype skill, akan memanggil prefab yg menjadi core dalan menjalan kan efek skill nya]

# BattleMaster.cs
sebagai jembatan DeckBuilderMaster.cs, Card.cs, TurnBaseSystem.cs, WinLoseSystem.cs dan mejalan beberapa perintah lain nya sperti menyimpan data player dan enemy. mengecek berapa jumlah enemy yg tersisah.}

# TurnBaseSystem.cs
system turnbase dijalan kan disini, akan mengecek giliran siapa dan menjalankan turn mereka

PlayTurn()
di gunakan untuk memulai turn.
disini setiap di jalankan akan mengecek jumlah pemain ( player + jumlah total enemy saat ini), jika turn saat ini lebih besar sama dengan jumlah pemain maka akan masuk round baru dan turn saat ini akan di reset ke 0. 
lalu akan mengecek lagi jika (currentTurn == 0) maka ini adalah giliran pemain dan akan mengakses 'deckBuilderMaster.cs' untuk mendraw kartu player
jika (currentTurn != 0) maka ini giliran enemy, dan akan mengakses 'EnemyBrain.cs' untuk mejalan kan giliran mereka

NewRound()
tempat untuk mereset turn dan memulai round baru. disini juga akan melakukan pengecekan buff dan debuff yg di miliki setiap pemain, sperti apakah effect nya sudah selesai, atau menjalankan dot efek

RemoveEnemy(EnemyBody enemyBody)
ini untuk mengurangi jumlah enemy dan mengupdate max turn nya

# EnemyBrain.cs
tempat enemy mejalankan logic sederhana mereka. disini sistem nya masih sederhan seperti membuat kartu. lalu akses kartu tersebut secara random dengan limit energy dan batas kartu di tangan adalah 5

CreateCard()
ini tempat untuk menciptakan 'Card', lalu di hide di dalam enemy tersebut

PlayThisTurn()
ini untuk menjalan kan action 'Card', dengan cara memanggil random 'Card' yg sudah dibuat, di sini akan terus melakukan perulangan hingga energy habis atau cardOnHand habis.
jika masih bisa menjalan kan 'Card' maka akan mengakses 'Card.ActionCard()'
jika sudah tidak bisa menjalan kan turn nya maka akan mengakses 'TurnBaseSystem.PlayNextTurn()'

# BuffDebuffHandler.cs
system buff and debuff untuk menyimpan efek yg di terima setiap karakter.
untuk penyimpanan efek nya di simpan dalam dictionary agar list efek bisa di kelompokan dengan mudah berdasarkan BuffDebuffData nya.

TakeEffect(BuffDebuffData buffDebuffData, int effectAmount, int roundLife)
tempat membuat dan menyimpan efek ke dalam dictionary lalu merubah stats di 'ChangeStats'

ChangeStats(BuffDebuffData buffDebuffData, int amount, bool isAddEffect = true)
tempatn untuk merubah stats character, 'isAddEffect' berperan apakah ini sedang menambahakan efek atau mengembalikan efek,
misal buff armoclass +4 jika isAddEffect = true maka armorClass +4 tapi jika isAddEffect =fale maka armorClass -4

TakeDotDamage(BuffDebuffData buffDebuffData)
di sini untu menjalan dot damge yg di jalan kan setiap new rount

CheckAllEffect()
ini berperan untuk mengecek setiap efek yg ada, seperti mengecek roundLife setiap efek apakah sudah habis atau belum. jika sudah maka efek akan di remove dari list di dictionary dan mengembalikan kembali status nya 'ChangeStats'. disini juga menjalankan 'TakeDotDamage'
