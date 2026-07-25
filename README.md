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

di game ini akurasi dan damage berdasrkan putaran dadu, dengan aturan 2d6 -> berarti 2 dadu dengan masing2 daru bernilai maksimal 6 dengan output range (2-12)

target utama saat ini adalah bagaiman sistem deck bekerja, efek kartu yg bisa di berikan yaitu memberi damage, efek buff dan debuff, dan juga dot.

# What I Would Do With More Time
untuk kedepan direncanakan setiap lanjut ke stage berikut nya pemain akan memilih bonus stats tambahan, pembuatan jenis efek skill baru seperti yg simple increase die roll setiap 2 turn, atau disable jenis kartu tertentu.

# Known Issues
-> ketika select card ada kemungkinan kecil ketika di klik lagi malah menjalan kan action nya, ini sudah aku coba perbaiki di raycast kursor nya tapi belum yakin apakah itu dapat menyelsaikan masalah ini, untuk saat ini belum menemukan masalah ini lagi
-> move card, ketika kartu di select lalu balik ketangan urutan nya berubah, ini karena untuk sekarang kartu hanyak balik ke transform parent nya tapi tidak ke susunan child nya
-> magic missile, efek kartu untuk saat ini hanya berefek ke kartu di tangan saja tidak di kartu di deck. misal pakai 1 kali magic missile harusnya kartu berikut nya memiliki dmg roll 1d5 tapi jika kartu di ambil dari deck balik ke dmg roll awal 1d2
