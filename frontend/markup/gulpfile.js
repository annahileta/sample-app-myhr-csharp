const gulp        = require('gulp'),
    browserSync   = require('browser-sync').create(),
    autoprefixer  = require('gulp-autoprefixer'),
    uglify        = require("gulp-uglify"),
    sass          = require('gulp-sass'),
    cleanCSS      = require('gulp-clean-css'),
    rename        = require("gulp-rename"),
    newer         = require("gulp-newer"),
    plumber       = require('gulp-plumber'),
    strip         = require('gulp-strip-comments'),
    nunjucks      = require('gulp-nunjucks'),
    groupqueries  = require('gulp-group-css-media-queries'),
    svgSprite     = require('gulp-svg-sprite'),
    watch         = require('gulp-watch'),
    webp          = require('gulp-webp'),
    webpack       = require('webpack'),
    webpackStream = require('webpack-stream');
    clean         = require("gulp-clean");

const paths = {
    templates: {
        src: 'src/templates/*.html',
        dest: 'build',
        watch: 'src/templates/**/*.html',
    },
    styles: {
        src: 'src/assets/scss/main.scss',
        dest: 'build/assets/css',
        watch: 'src/assets/scss/**/*.scss'
    },
    scripts: {
        src: 'src/assets/js/main.js',
        libs: 'src/assets/js/libs/*.js',
        dest: 'build/assets/js',
        watch: 'src/assets/js/*.js'
    },
    images: {
        src: 'src/assets/img/*',
        dest: 'build/assets/img'
    },
    media: {
        src: 'src/assets/media/*',
        dest: 'build/assets/media'
    },
    fonts: {
        src: 'src/assets/fonts/**/*',
        dest: 'build/assets/fonts'
    },
    sprite: {
        src: 'src/assets/img/svg-icons/*.svg',
        dest: 'build/assets/img/sprite'
    }
};

gulp.task("templates", function() {
    return gulp.src(paths.templates.src)
        .pipe(nunjucks.compile({}, {
            autoescape: true,
            watch: true
        }))
        .pipe((strip()))
        .pipe(gulp.dest(paths.templates.dest))
        .on("end", browserSync.reload);
});

gulp.task("styles", function() {
    return gulp.src(paths.styles.src)
        .pipe(plumber())
        .pipe(sass())
        .pipe(groupqueries())
        .pipe(autoprefixer({
            browsers: ['last 3 versions'],
            cascade: false
        }))
        .pipe(cleanCSS())
        .pipe(rename({suffix: ".min"}))
        .pipe(gulp.dest(paths.styles.dest))
        .on("end", browserSync.reload);
});

gulp.task("scripts", function () {
    return gulp.src(paths.scripts.src)
        .pipe(webpackStream({
            mode: 'development',
            output: {
                filename: 'main.js',
            },
            module: {
                rules: [
                    {
                        test: /\.(js)$/,
                        exclude: /(node_modules)/,
                        loader: 'babel-loader',
                        query: {
                            presets: ['@babel/env']
                        }
                    }
                ]
            }
        }))
        .pipe(uglify())
        // .pipe(rename({suffix: ".min"}))
        .pipe(gulp.dest(paths.scripts.dest))
        .on("end", browserSync.reload);
});

gulp.task("jsLibs", function () {
    return gulp.src(paths.scripts.libs)
        .pipe(uglify())
        .pipe(rename({suffix: ".min"}))
        .pipe(gulp.dest(paths.scripts.dest))
        .on("end", browserSync.reload);
});

gulp.task("images", function() {
    return gulp.src(paths.images.src)
        .pipe(newer(paths.images.dest))
        .pipe(gulp.dest(paths.images.dest))
        .pipe(webp({quality: 90}))
        .pipe(gulp.dest(paths.images.dest))
        .on("end", browserSync.reload);
});

gulp.task("media", function() {
    return gulp.src(paths.media.src)
        .pipe(gulp.dest(paths.media.dest))
        .on("end", browserSync.reload);
});

gulp.task("fonts", function() {
    return gulp.src(paths.fonts.src)
        .pipe(gulp.dest(paths.fonts.dest))
        .on("end", browserSync.reload);
});

gulp.task("watch", function() {
    return new Promise((res, rej) => {
        watch(paths.templates.watch, gulp.series("templates"));
        watch(paths.styles.watch, gulp.series("styles"));
        watch(paths.images.src, gulp.series("images"));
        watch(paths.scripts.watch, gulp.series("scripts"));
        res();
    });
});

gulp.task("clean", function() {
    return gulp.src(["./build/*"],
        {read: false})
        .pipe(clean({force: true}));
});

gulp.task("serve", function () {
    return new Promise((res, rej) => {
        browserSync.init({
            server: "./build/",
            tunnel: false,
            port: 4200
        });
        res();
    });
});

gulp.task("svgsprite", function() {
    return gulp.src(paths.sprite.src) // svg files for sprite
        .pipe(svgSprite({
                mode: {
                    stack: {
                        sprite: "../sprite.svg"  //sprite file name
                    }
                },
            }
        ))
        .pipe(gulp.dest(paths.sprite.dest));
});

/*
* Default task
* */
gulp.task("default", gulp.series("clean",
    gulp.parallel("templates", "styles", "images", "media", "scripts", "fonts", "svgsprite", "jsLibs"),
    gulp.parallel("watch", "serve")
));
