docker build -t monster-desciption-image .

docker tag monster-desciption-image registry.heroku.com/monster-description/web

docker push registry.heroku.com/monster-description/web

heroku container:release web -a monster-description