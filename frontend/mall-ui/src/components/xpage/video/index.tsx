import u from '@/utils';
import { IPageVideoItem } from '@/utils/models';
import React from 'react';
import videojs, { VideoJsPlayer } from 'video.js';
import 'video.js/dist/video-js.css';

export default function IndexPage(props: { data: IPageVideoItem }) {
  const {
    data,
    data: { type },
  } = props;

  const [ready, _ready] = React.useState(false);
  const videoRef = React.useRef<HTMLVideoElement | null>(null);
  const playerRef = React.useRef<VideoJsPlayer | null>(null);

  React.useEffect(() => {
    if (videoRef.current != null && playerRef.current == null) {
      playerRef.current = videojs(
        videoRef.current,
        {
          autoplay: false,
          controls: true,
          responsive: true,
          fluid: true,
        },
        () => {
          //ready
          _ready(true);
        },
      );
    }
  }, [videoRef]);

  React.useEffect(() => {
    if (playerRef.current != null && !u.isEmpty(data.url) && ready) {
      playerRef.current.src(data.url || '');
    }
  }, [playerRef, data, ready]);

  // Dispose the Video.js player when the functional component unmounts
  React.useEffect(() => {
    return () => {
      if (playerRef.current && !playerRef.current.isDisposed()) {
        playerRef.current.dispose();
      }
    };
  }, []);

  if (type != 'video') {
    return null;
  }

  return (
    <>
      <div>
        <video className="video-js vjs-big-play-centered" ref={videoRef} />
      </div>
    </>
  );
}
