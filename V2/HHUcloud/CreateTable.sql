CREATE TABLE "ALBUM" (
    -- 专辑列表

    -- 专辑ID
    "AlbumId" INTEGER NOT NULL CONSTRAINT "PK_ALBUM" PRIMARY KEY AUTOINCREMENT,

    -- 标题
    "Title" TEXT NOT NULL,

    -- 发行时间
    "Published" TEXT NOT NULL,

    -- 封面链接
    "CoverLink" TEXT NOT NULL
);

CREATE TABLE "ALBUM_HAS_SONG" (
    -- 专辑的歌曲列表

    -- 专辑ID
    "AlbumId" INTEGER NOT NULL,

    -- 歌曲ID
    "SongId" INTEGER NOT NULL,
    CONSTRAINT "PK_ALBUM_HAS_SONG" PRIMARY KEY ("AlbumId", "SongId")
);

CREATE TABLE "ARTIST" (
    -- 歌手列表

    -- 歌手ID
    "ArtistId" INTEGER NOT NULL CONSTRAINT "PK_ARTIST" PRIMARY KEY AUTOINCREMENT,

    -- 歌手名
    "Name" TEXT NOT NULL,

    -- 简介
    "Description" TEXT NULL
);

CREATE TABLE "ARTIST_HAS_ALBUM" (
    -- 歌手的专辑列表

    -- 歌手ID
    "ArtistId" INTEGER NOT NULL,

    -- 专辑ID
    "AlbumId" INTEGER NOT NULL,
    CONSTRAINT "PK_ARTIST_HAS_ALBUM" PRIMARY KEY ("ArtistId", "AlbumId")
);

CREATE TABLE "ARTIST_HAS_SONG" (
    -- 歌手的歌曲列表

    -- 歌手ID
    "ArtistId" INTEGER NOT NULL,

    -- 歌曲ID
    "SongId" INTEGER NOT NULL,
    CONSTRAINT "PK_ARTIST_HAS_SONG" PRIMARY KEY ("ArtistId", "SongId")
);

CREATE TABLE "PLAYLIST" (
    -- 歌单列表

    -- 歌单ID
    "PlaylistId" INTEGER NOT NULL CONSTRAINT "PK_PLAYLIST" PRIMARY KEY AUTOINCREMENT,

    -- 标题
    "Title" TEXT NOT NULL,

    -- 创建时间
    "Created" TEXT NOT NULL,

    -- 用户ID
    "UserId" INTEGER NOT NULL
);

CREATE TABLE "PLAYLIST_HAS_SONG" (
    -- 歌单的歌曲列表

    -- 歌单ID
    "PlaylistId" INTEGER NOT NULL,

    -- 歌曲ID
    "SongId" INTEGER NOT NULL,

    -- 添加时间
    "Added" TEXT NOT NULL,
    CONSTRAINT "PK_PLAYLIST_HAS_SONG" PRIMARY KEY ("PlaylistId", "SongId")
);

CREATE TABLE "PLAYRECORD" (
    -- 播放记录

    -- 用户ID
    "UserId" INTEGER NOT NULL,

    -- 歌曲ID
    "SongId" INTEGER NOT NULL,

    -- 播放时间
    "Played" TEXT NOT NULL,

    -- 播放次数
    "Count" INTEGER NOT NULL,
    CONSTRAINT "PK_PLAYRECORD" PRIMARY KEY ("UserId", "SongId")
);

CREATE TABLE "SONG" (
    -- 歌曲列表

    -- 歌曲Id
    "SongId" INTEGER NOT NULL CONSTRAINT "PK_SONG" PRIMARY KEY AUTOINCREMENT,

    -- 是否可用
    "Accessible" INTEGER NOT NULL,

    -- 点击数
    "Count" INTEGER NOT NULL,

    -- 歌曲名
    "Title" TEXT NOT NULL,

    -- 时长
    "Duration" TEXT NOT NULL,

    -- 专辑Id
    "AlbumId" INTEGER NOT NULL
);

CREATE TABLE "USER" (
    -- 用户表

    -- 用户Id
    "UserId" INTEGER NOT NULL CONSTRAINT "PK_USER" PRIMARY KEY AUTOINCREMENT,

    -- 用户名
    "Name" TEXT NULL,

    -- 生日
    "Birthday" TEXT NULL,

    -- 邮箱地址
    "Email" TEXT NOT NULL,

    -- 密码
    "UserPassword" TEXT NOT NULL,

    -- 安全码
    "SecurityKey" TEXT NULL,

    -- 用户角色
    "UserRole" INTEGER NOT NULL,

    -- 创建日期
    "Created" TEXT NOT NULL,

    -- 头像编号
    "ProfilePhoto" INTEGER NOT NULL
);

