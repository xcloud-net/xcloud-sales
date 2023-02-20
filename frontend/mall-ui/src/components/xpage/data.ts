import { IPageData } from '@/utils/models';

const pagedata: IPageData = {
  title: '首页',
  desc: '',
  items: [
    {
      id: 100,
      type: 'video',
      url: 'https://www.runoob.com/try/demo_source/movie.mp4',
    },
    {
      id: 0,
      type: 'slider',
      sliders: [
        {
          img: 'https://mui.com/static/images/cards/paella.jpg',
          text: '这是一个 Swiper1',
          link: { goodsId: 3 },
        },
        {
          img:
            'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&w=350&dpr=2',
          text: '这是一个 Swiper2',
          link: { couponId: '' },
        },
        {
          img: 'https://mui.com/static/images/cards/contemplative-reptile.jpg',
          text: '这是一个 Swiper3',
        },
        {
          img: 'https://mui.com/static/images/cards/paella.jpg',
          text: '这是一个 Swiper1',
        },
        {
          img:
            'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&w=350&dpr=2',
          text: '这是一个 Swiper2',
        },
        {
          img: 'https://mui.com/static/images/cards/contemplative-reptile.jpg',
          text: '这是一个 Swiper3',
        },
        {
          img: 'https://mui.com/static/images/cards/paella.jpg',
          text: '这是一个 Swiper1',
        },
        {
          img:
            'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&w=350&dpr=2',
          text: '这是一个 Swiper2',
        },
        {
          img: 'https://mui.com/static/images/cards/contemplative-reptile.jpg',
          text: '这是一个 Swiper3',
        },
        {
          img: 'https://mui.com/static/images/cards/paella.jpg',
          text: '这是一个 Swiper1',
        },
        {
          img:
            'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&w=350&dpr=2',
          text: '这是一个 Swiper2',
        },
        {
          img: 'https://mui.com/static/images/cards/contemplative-reptile.jpg',
          text: '这是一个 Swiper3',
        },
      ],
    },
    {
      id: 3,
      type: 'picture',
      picture: 'https://mui.com/static/images/cards/contemplative-reptile.jpg',
      link: { goodsId: 9 },
    },
    {
      id: 5,
      type: 'goods-collection',
      displayType: 'grid',
      goodsIds: [38, 39, 40],
    },
    {
      id: 6,
      type: 'content',
      content: '# 这是一个内容 \n\n > test',
    },
  ],
};

export default pagedata;
