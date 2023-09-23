FROM alpine:latest

# Download Latest SongDetailsCache

RUN curl -s https://api.github.com/repos/kinsi55/BeatSaber_SongDetails/releases/latest \
  | grep "browser_download_url.*zip" \
  | cut -d : -f 2,3 \
  | tr -d \" \
  | wget -qi -
  
