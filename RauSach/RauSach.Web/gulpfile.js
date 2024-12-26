/// <binding BeforeBuild='default' />
var path = require('path'),
    gulp = require('gulp'),
    uglify = require('gulp-uglify'),
    concat = require('gulp-concat'),
    cleanCSS = require('gulp-clean-css'),
    replace = require('gulp-replace'),
    clean = require('gulp-clean');

var basePath = path.resolve(__dirname, "wwwroot");

gulp.task("clean", function () {
    return gulp.src(path.resolve(basePath, 'build/*'), { read: false, allowEmpty: true })
        .pipe(clean());
});

gulp.task('js', function (done) {
    // array of all the js paths you want to bundle.
    var scriptSources = [
        //path.resolve(basePath, 'assets/js/jquery/jquery.js'),
        //path.resolve(basePath, 'assets/js/jquery/jquery-ui.js'),
        path.resolve(basePath, 'assets/js/jquery/jquery.plugin.min.js'),
        path.resolve(basePath, 'assets/js/jquery/jquery.countdown.min.js'),
        path.resolve(basePath, 'assets/js/jquery/jquery.appear.js'),
        path.resolve(basePath, 'assets/js/*.js'),
        path.resolve(basePath, 'assets/js/themepunch/jquery.themepunch.tools.min.js'),
        path.resolve(basePath, 'assets/js/themepunch/jquery.themepunch.revolution.min.js'),
        path.resolve(basePath, 'assets/js/extensions/*.js'),
        path.resolve(basePath, 'assets/js/theme/*.js')
    ];
    gulp.src(scriptSources)
        // name of the new file all your js files are to be bundled to.
        .pipe(concat('bundle.js'))
        .pipe(uglify())
        // the destination where the new bundled file is going to be saved to.
        .pipe(gulp.dest(path.resolve(basePath, 'build')));
    done();
});

gulp.task('css', function () {
    var cssSources = [
        //path.resolve(basePath, 'assets/css/bootstrap.css'),
        path.resolve(basePath, 'assets/css/animate.css'),
        path.resolve(basePath, 'assets/css/themewar-font.css'),
        path.resolve(basePath, 'assets/css/organio-icon.css'),
        path.resolve(basePath, 'assets/css/owl.carousel.min.css'),
        path.resolve(basePath, 'assets/css/owl.theme.default.min.css'),
        //path.resolve(basePath, 'assets/css/nice-select.css'),
        path.resolve(basePath, 'assets/css/slick.css'),
        path.resolve(basePath, 'assets/css/lightcase.css'),
        path.resolve(basePath, 'assets/css/settings.css'),
        path.resolve(basePath, 'assets/css/preset.css'),
        path.resolve(basePath, 'assets/css/ignore_for_wp.css'),
        path.resolve(basePath, 'assets/css/theme.css'),
        path.resolve(basePath, 'assets/css/responsive.css'),
        path.resolve(basePath, 'css/*.css')];
    return gulp.src(cssSources)
        .pipe(concat('style.min.css'))
        .pipe(replace(/url\(..\/images\//gi, 'url\(../assets/images/'))
        .pipe(replace(/url\(..\/loader.gif\)/g, 'url(../assets/loader.gif)'))
        .pipe(replace(/url\(\'..\/fonts\//gi, 'url(\'../assets/fonts/'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(gulp.dest(path.resolve(basePath, 'build')));
});

gulp.task('default', gulp.series('clean', 'js', 'css'))
